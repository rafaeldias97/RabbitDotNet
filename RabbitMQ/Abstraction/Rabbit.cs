using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Interfaces;
using RabbitMQ.Models;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace RabbitMQ.Abstraction
{
    public class Rabbit : IRabbitMQ
    {
        private readonly Config config;

        private string replyQueueName;
        private EventingBasicConsumer consumer;
        private BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private IBasicProperties props;

        public Rabbit(Config config)
        {
            this.config = config;
        }

        public void Publisher(string queue, string message)
        {
            var factory = config.CreateConnection();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                                     durable: config.durable,
                                     exclusive: config.exclusive,
                                     autoDelete: config.autoDelete,
                                     arguments: config.arguments);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queue,
                                     basicProperties: null,
                                     body: body);
                System.Console.WriteLine(" [x] Enviando {0}", message);
            }
        }

        // Call Consumer
        public void On(string name, Action<string> cb)
        {
            Thread thread = new Thread(() => Consumer(name, cb));
            thread.Start();
        }

        private void Consumer(string name, Action<string> cb)
        {
            var factory = config.CreateConnection();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: name,
                                     durable: config.durable,
                                     exclusive: config.exclusive,
                                     autoDelete: config.autoDelete,
                                     arguments: config.arguments);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    System.Console.WriteLine(ea.Body);
                    cb.Invoke(Encoding.UTF8.GetString(ea.Body));
                };

                channel.BasicConsume(queue: name,
                                     autoAck: true,
                                     consumer: consumer);

                System.Console.ReadKey();
            }
        }

        public string RPCPublisher(string queue, string message)
        {
            var res = "";
            Thread t = new Thread(() => res = RPCPublisherThread(queue, message));
            t.Start();
            t.Join();
            return res;
        }
        private string RPCPublisherThread(string queue, string message)
        {
            var factory = config.CreateConnection();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                replyQueueName = channel.QueueDeclare().QueueName;
                consumer = new EventingBasicConsumer(channel);

                props = channel.CreateBasicProperties();
                var correlationId = Guid.NewGuid().ToString();
                props.CorrelationId = correlationId;
                props.ReplyTo = replyQueueName;

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var response = Encoding.UTF8.GetString(body);
                    if (ea.BasicProperties.CorrelationId == correlationId)
                    {
                        respQueue.Add(response);
                    }
                };

                var messageBytes = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(
                    exchange: "",
                    routingKey: queue,
                    basicProperties: props,
                    body: messageBytes);

                channel.BasicConsume(
                    consumer: consumer,
                    queue: replyQueueName,
                    autoAck: true);

                return respQueue.Take();
            }
        }

        public void RPCConsumer(string name, Func<string, string> cb)
        {
            Thread t = new Thread(() => RPCConsumerThread(name, cb));
            t.Start();
        }
        private void RPCConsumerThread(string name, Func<string, string> cb)
        {
            var factory = config.CreateConnection();
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: name, durable: config.durable,
                  exclusive: config.exclusive, autoDelete: config.autoDelete, arguments: config.arguments);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: name,
                  autoAck: false, consumer: consumer);

                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body);
                        response = cb(message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                        response = "";
                        response = cb(response);
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                          basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                          multiple: false);
                    }
                };
                System.Console.ReadLine();
            }
        }
    }
}

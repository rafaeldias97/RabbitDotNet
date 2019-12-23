using FluentAssertions;
using RabbitMQ.Abstraction;
using RabbitMQ.Interfaces;
using RabbitMQ.Models;
using System.Threading;
using Xunit;

namespace Test
{
    public class RabbitMQTest
    {
        private readonly IRabbitMQ rb;
        public RabbitMQTest()
        {
            rb = new Config(
                    hostName: "localhost",
                    port: 5672,
                    password: "Admin123XX_",
                    user: "mqadmin"
                ).Start();
        }

        [Theory]
        [InlineData("test1")]
        [InlineData("second message")]
        public void Publisher_Consumer(string message)
        {
            Thread publisher = new Thread(() =>
            {
                rb.Publisher("queue.topic", message);
            });


            rb.On("queue.topic", (res) =>
            {
                res.Should().Be(message, $"The message should be {message}");
            });

            publisher.Start();
            //System.Console.ReadKey();
        }

        [Theory]
        [InlineData("test1", "retorno1")]
        [InlineData("second message", "retorno 2")]
        public void RPCPublisherConsumer(string message, string callback)
        {
            Thread t1 = new Thread(() => { 
                rb.RPCConsumer("queuerpc.topic", (res) =>
                {
                    res.Should().Be(message, $"The message should be {message}");
                    return callback;
                });
            });

            Thread t2 = new Thread(() =>
            {
                var res = rb.RPCPublisher("queuerpc.topic", message);
                res.Should().Be(callback, $"The callback should be {callback}");
            });

            t1.Start();
            t2.Start();
        }
    }
}

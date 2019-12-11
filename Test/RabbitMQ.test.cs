using RabbitMQ.Abstraction;
using RabbitMQ.Interfaces;
using RabbitMQ.Models;
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

        [Fact]
        public void Consumer()
        {
            rb.On("queue.topic", (res) =>
            {
                Assert.Equal(res, "message");
            });
        }

        [Fact]
        public void Publisher()
        {
            rb.publisher("queue.topic", "message");
        }

        [Fact]
        public void RPCConsumer()
        {
            rb.rpcConsumer("queuerpc.topic", (res) =>
            {
                Assert.Equal(res, "message to consumer rpc");
                return "response to sender";
            });
        }

        [Fact]
        public void RPCPublisher()
        {
            var res = rb.rpcPublisher("queuerpc.topic", "message to consumer rpc");
            Assert.Equal(res, "response to sender");
        }
    }
}

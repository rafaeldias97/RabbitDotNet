using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Abstraction;
using RabbitMQ.Interfaces;
using RabbitMQ.Models;

namespace RabbitMQ.Abstraction
{
    public static class Operator
    {
        public static IRabbitMQ Start(this Config config)
        {
            var serv = new ServiceCollection()
              .AddSingleton<IRabbitMQ, Rabbit>((x) => new Rabbit(config))
              .BuildServiceProvider();
           return serv.GetService<IRabbitMQ>();
        }
    }
}

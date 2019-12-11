using RabbitMQ.Client;
using RabbitMQ.Models;
using System;

namespace RabbitMQ.Abstraction
{
    internal static class Connection
    {
        public static ConnectionFactory CreateConnection(this Config config)
        {
            return new ConnectionFactory()
            {
                HostName = config.hostName,
                UserName = config.user,
                Password = config.password,
                Port = config.port
            };
        }
    }
}

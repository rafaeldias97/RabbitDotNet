using RabbitMQ.Models;
using System;

namespace RabbitMQ.Interfaces
{
    public interface IRabbitMQ
    {
        void Publisher(string queue, string message);
        void On(string name, Action<string> cb);
        string RPCPublisher(string queue, string message);
        void RPCConsumer(string name, Func<string, string> cb);
    }
}

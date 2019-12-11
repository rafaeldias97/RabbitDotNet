using RabbitMQ.Models;
using System;

namespace RabbitMQ.Interfaces
{
    public interface IRabbitMQ
    {
        void publisher(string queue, string message);
        void On(string name, Action<string> cb);
        string rpcPublisher(string queue, string message);
        void rpcConsumer(string name, Func<string, string> cb);
    }
}

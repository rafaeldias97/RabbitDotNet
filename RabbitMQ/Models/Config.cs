using RabbitMQ.Interfaces;
using System.Collections.Generic;

namespace RabbitMQ.Models
{
    public class Config
    {
        public Config(string hostName, int port, string user, string password)
        {
            this.hostName = hostName;
            this.port = port;
            this.user = user;
            this.password = password;
        }

        public Config(string hostName)
        {
            this.hostName = hostName;
        }

        public Config(string hostName, int port)
        {
            this.hostName = hostName;
            this.port = port;
        }

        public Config(string hostName, int port, string user, string password, bool durable, bool exclusive, bool autoDelete)
        {
            this.hostName = hostName;
            this.port = port;
            this.user = user;
            this.password = password;
            this.durable = durable;
            this.exclusive = exclusive;
            this.autoDelete = autoDelete;
        }

        public string hostName { get; set; } = "localhost";
        public int port { get; set; } = 5672;
        public string user { get; set; }
        public string password { get; set; }
        public bool durable { get; set; } = true;
        public bool exclusive { get; set; } = false;
        public bool autoDelete { get; set; } = false;
        public Dictionary<string, object> arguments { get; set; } = null;
    }
}

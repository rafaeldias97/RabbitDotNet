using RabbitMQ.Abstraction;
using RabbitMQ.Models;
using System;

namespace PublisherExample
{
    public class Publisher
    {
        static void Main(string[] args)
        {
            var rb = new Config(
                    hostName: "localhost",
                    port: 5672,
                    password: "Admin123XX_",
                    user: "mqadmin"
                ).Start();

            // Exemplo Publisher
            string mensagem = "ola";
            rb.Publisher("teste", mensagem);

            // RPC Publisher
            var res = rb.RPCPublisher("teste.a", mensagem);
            Console.WriteLine(res);
        }
    }
}

using RabbitMQ.Abstraction;
using RabbitMQ.Models;
using System;

namespace PublisherApp
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
            rb.publisher("teste", mensagem);

            // RPC Publisher
            var res = rb.rpcPublisher("teste.a", mensagem);
            Console.WriteLine(res);
        }
    }
}

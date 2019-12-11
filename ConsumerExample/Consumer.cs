using RabbitMQ.Abstraction;
using RabbitMQ.Models;
using System;

namespace ConsumerExample
{
    class ConsumerApp
    {
        static void Main(string[] args)
        {
            var rb = new Config(
                    hostName: "localhost",
                    port: 5672,
                    password: "Admin123XX_",
                    user: "mqadmin"
                ).Start();


            // Exemplo Consumer
            rb.On("teste", (x) =>
            {
                Console.WriteLine(x);
            });

            // Consumer RPC
            rb.rpcConsumer("teste.a", (x) =>
            {
                Console.WriteLine(x);
                return "KKKK eu sou o melhor";
            });

            Console.ReadKey();
        }
    }
}

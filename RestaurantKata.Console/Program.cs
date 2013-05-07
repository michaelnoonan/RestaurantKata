using System;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestaurantKata.Infrastructure;

namespace RestaurantKata
{
    class Program
    {
        static void Main(string[] args)
        {
            var cashier = new ThreadedConsumer<Cashier>(new Cashier(new ConsoleOrderConsumerProcessor()));
            var assistantManager = new ThreadedConsumer<AssistantManager>(new AssistantManager(cashier));
            var cook = new ThreadedConsumer<Cook>(new Cook(assistantManager));
            var waitress = new Waitress("Sexy Mary", cook);

            cashier.Start();
            assistantManager.Start();
            cook.Start();

            for (int i = 0; i < 2; i++)
            {
                waitress.PlaceOrder(i, i % 2 == 0 ? "good looking" : "dodgy", new[]
                                                     {
                                                         new Item("Sushi", 2),
                                                         new Item("Clean glass", 2),
                                                         new Item("Sake", 2),
                                                     });
            }

            Thread.Sleep(5000);

            for (int i = 0; i < 2; i++)
            {
                cashier.Consumer.PayBill(i, 100M);
                Console.WriteLine("Paid bill for table: " + i);
            }

            Console.ReadLine();
        }
    }

    public class ConsoleOrderConsumerProcessor : IOrderConsumer
    {
        public void Consume(Order order)
        {
            var settings = new JsonSerializerSettings
                               {
                                   Formatting = Formatting.Indented,
                                   ContractResolver = new CamelCasePropertyNamesContractResolver()
                               };
            
            Console.WriteLine(JsonConvert.SerializeObject(order, settings));
        }
    }

}

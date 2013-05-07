using System;
using System.Collections.Generic;
using System.Linq;
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
            var threadedCashier = new ThreadedConsumer<Cashier>(new Cashier(new ConsoleOrderConsumerProcessor()));
            var threadedAssistantManager = new ThreadedConsumer<AssistantManager>(new AssistantManager(threadedCashier));
            var threadedCooks = new List<IOrderConsumer>();
            const int numberOfCooks = 6;
            for (int i = 0; i < numberOfCooks; i++)
            {
                threadedCooks.Add(new ThreadedConsumer<Cook>(new Cook(threadedAssistantManager)));
            }
                
            var roundRobinCook = new ThreadedConsumer<RoundRobinConsumer>(new RoundRobinConsumer(threadedCooks));
            var waitress = new Waitress("Sexy Mary", roundRobinCook);
            
            roundRobinCook.Start();
            foreach (var cook in threadedCooks.OfType<IStartable>()) cook.Start();
            threadedCashier.Start();
            threadedAssistantManager.Start();
            const int numberOfOrders = 200;
            for (int i = 0; i < numberOfOrders; i++)
            {
                waitress.PlaceOrder(i, i % 2 == 0 ? "good looking" : "dodgy", new[]
                                                     {
                                                         new Item("Sushi", 2),
                                                         new Item("Clean glass", 2),
                                                         new Item("Sake", 2),
                                                     });
            }

            Thread.Sleep(10000);

            for (int i = 0; i < numberOfOrders; i++)
            {
                while (!threadedCashier.Consumer.IsBillReadyToPay(i))
                {
                    Thread.Sleep(100);
                }
                {
                    threadedCashier.Consumer.PayBill(i, 100M);
                    Console.WriteLine("Paid bill for table: " + i);
                }
            }

            Console.WriteLine("Press enter to finish...");
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

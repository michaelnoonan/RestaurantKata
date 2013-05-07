using System;
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
            var threadedCook1 = new ThreadedConsumer<Cook>(new Cook(threadedAssistantManager));
            var threadedCook2 = new ThreadedConsumer<Cook>(new Cook(threadedAssistantManager));
            var threadedCook3 = new ThreadedConsumer<Cook>(new Cook(threadedAssistantManager));
            threadedCook1.Start();
            threadedCook2.Start();
            threadedCook3.Start();
            var roundRobinCook = new ThreadedConsumer<RoundRobinConsumer>(
                new RoundRobinConsumer(
                    threadedCook1,
                    threadedCook2,
                    threadedCook3));
            roundRobinCook.Start();
            var waitress = new Waitress("Sexy Mary", roundRobinCook);

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

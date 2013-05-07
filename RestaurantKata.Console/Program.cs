using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestaurantKata.Infrastructure;

namespace RestaurantKata
{
    class Program
    {
        static void Main()
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
            const int numberOfOrders = 20;
            var startTimes = new Dictionary<string, DateTime>();
            
            for (var i = 0; i < numberOfOrders; i++)
            {
                var id = waitress.PlaceOrder(i, i % 2 == 0 ? "good looking" : "dodgy", new[]
                                                     {
                                                         new Item("Sushi", 2),
                                                         new Item("Clean glass", 2),
                                                         new Item("Sake", 2),
                                                     });
                startTimes.Add(id, DateTime.Now);
            }

            var ordersPaid = 0;
            var buffer = new StringBuilder();
            do
            {
                var ordersToPay = threadedCashier.Consumer.GetOrdersReadyToPay();
                foreach (var order in ordersToPay)
                {
                    threadedCashier.Consumer.PayBill(order);
                    var startTime = startTimes[order.Id];
                    buffer.AppendFormat("Order paid. Time: {0} Id {1} {2}", (DateTime.Now - startTime) ,  order.Id, Environment.NewLine);
                    ordersPaid++;
                }

            } while (ordersPaid < numberOfOrders);

            Thread.Sleep(1000);
            Console.WriteLine(buffer.ToString());
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

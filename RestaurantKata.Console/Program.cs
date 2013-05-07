using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RestaurantKata.Infrastructure;

namespace RestaurantKata
{
    class Program
    {
        static void Main()
        {
            var monitor = new QueueMonitor();

            var cashier = new Cashier(new ConsoleOrderConsumerProcessor());
            var threadedCashier = new ThreadedConsumer<IOrderConsumer>(cashier);
            var threadedAssistantManager = new ThreadedConsumer<IOrderConsumer>(new AssistantManager(threadedCashier));
            var threadedCooks = new List<ThreadedConsumer<IOrderConsumer>>();
            const int numberOfCooks = 3;
            var random = new Random();
            for (int i = 0; i < numberOfCooks; i++)
            {
                threadedCooks.Add(new ThreadedConsumer<IOrderConsumer>(new Cook(threadedAssistantManager)));
            }
                
            var roundRobinCook = new ThreadedConsumer<IOrderConsumer>(new RoundRobinConsumer(threadedCooks));
            var waitress = new Waitress("Sexy Mary", roundRobinCook);
            
            roundRobinCook.Start();
            foreach (var cook in threadedCooks.OfType<IStartable>()) cook.Start();
            threadedCashier.Start();
            threadedAssistantManager.Start();

            monitor.AddComponent(threadedCashier);
            monitor.AddComponent(threadedAssistantManager);
            monitor.AddComponents(threadedCooks);
            monitor.AddComponent(roundRobinCook);
            monitor.Start();

            const int numberOfOrders = 250;
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
                var ordersToPay = cashier.GetOrdersReadyToPay();
                foreach (var order in ordersToPay)
                {
                    cashier.PayBill(order);
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
}

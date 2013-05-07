using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RestaurantKata.Infrastructure;

namespace RestaurantKata
{
    class Program
    {
        static void Main()
        {                      
            var cashier = new Cashier(new ConsoleOrderConsumerProcessor());
            var threadedCashier = new ThreadedConsumer<IOrderConsumer>(cashier);
            var threadedAssistantManager = new ThreadedConsumer<IOrderConsumer>(new AssistantManager(threadedCashier));
            var threadedCooks = new List<ThreadedConsumer<IOrderConsumer>>();
            const int numberOfCooks = 3;
            for (var i = 0; i < numberOfCooks; i++)
            {
                var threadedCook = new ThreadedConsumer<IOrderConsumer>(new TimeToLiveHandler(new Cook(threadedAssistantManager)));
                threadedCooks.Add(threadedCook);
            }
                
            var dispatcher = new ThreadedConsumer<IOrderConsumer>(new OrderDispatcher(threadedCooks), int.MaxValue);
            var waitress = new Waitress("Sexy Mary", dispatcher);
            
            dispatcher.Start();
            foreach (var cook in threadedCooks) cook.Start();
            threadedCashier.Start();
            threadedAssistantManager.Start();

            var monitor = new QueueMonitor();
            monitor.AddComponent(threadedCashier);
            monitor.AddComponent(threadedAssistantManager);
            monitor.AddComponents(threadedCooks);
            monitor.AddComponent(dispatcher);
            monitor.Start();

            const int numberOfOrders = 200;
            var startTimes = new Dictionary<string, DateTime>();
            
            for (var i = 0; i < numberOfOrders; i++)
            {
                var orderId = waitress.PlaceOrder(i, i % 2 == 0 ? "good looking" : "dodgy", new[]
                {
                    new Item("Sushi", 2),
                    new Item("Clean glass", 2),
                    new Item("Sake", 2),
                });
                
                startTimes.Add(orderId, DateTime.Now);
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

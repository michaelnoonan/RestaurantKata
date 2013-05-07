using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RestaurantKata.Infrastructure;

namespace RestaurantKata
{
    class Program
    {
        private static readonly IList<ThreadedConsumer<IOrderConsumer>> ThreadedConsumers = new List<ThreadedConsumer<IOrderConsumer>>(); 

        static void Main()
        {                      
            var assistantManager = PrepareAssistantManager();

            var kitchen = PrepareKitchen();
           
            var waitress = new Waitress("Sexy Mary", new PublishingConsumer("OrderPlaced"));

            var cashier = new Cashier(new ConsoleOrderConsumerProcessor());
            var threadedCashier = PrepareThreadedCashier(cashier);

            TopicPubSub.Instance.Subscribe("OrderPlaced", kitchen);
            TopicPubSub.Instance.Subscribe("FoodPrepared", assistantManager);
            TopicPubSub.Instance.Subscribe("OrderTotalled", threadedCashier);

            Start();

            const int numberOfOrders = 200;
            var startTimes = new Dictionary<string, DateTime>();
            
            StartPlacingOrders(numberOfOrders, waitress, startTimes);

            var summary = PayOrders(cashier, startTimes, numberOfOrders);

            Thread.Sleep(1000);

            Console.WriteLine(summary.ToString());
            Console.WriteLine("Press enter to finish...");

            Console.ReadLine();
        }

        private static ThreadedConsumer<IOrderConsumer> PrepareThreadedCashier(Cashier cashier)
        {
            var threadedCashier = new ThreadedConsumer<IOrderConsumer>("Cashier", cashier);
            ThreadedConsumers.Add(threadedCashier);
            return threadedCashier;
        }

        private static ThreadedConsumer<IOrderConsumer> PrepareAssistantManager()
        {
            var assistantManager = new ThreadedConsumer<IOrderConsumer>("AssistantManager", new AssistantManager(new PublishingConsumer("OrderTotalled")));
            ThreadedConsumers.Add(assistantManager);
            return assistantManager;
        }

        private static void Start()
        {
            StartQueueMonitoring();

            foreach (var startable in ThreadedConsumers)
            {
                startable.Start();
            }
        }

        private static ThreadedConsumer<IOrderConsumer> PrepareKitchen()
        {
            var threadedCooks = new List<ThreadedConsumer<IOrderConsumer>>();
            const int numberOfCooks = 3;
            for (var i = 0; i < numberOfCooks; i++)
            {
                var threadedCook = new ThreadedConsumer<IOrderConsumer>("Cook", new TimeToLiveHandler(new Cook(new PublishingConsumer("FoodPrepared"))));
                threadedCooks.Add(threadedCook);
                ThreadedConsumers.Add(threadedCook);
            }

            var dispatcher = new ThreadedConsumer<IOrderConsumer>("Dispatcher", new OrderDispatcher(threadedCooks), int.MaxValue);
            ThreadedConsumers.Add(dispatcher);
            return dispatcher;
        }

        private static void StartQueueMonitoring()
        {
            var monitor = new QueueMonitor();

            monitor.AddComponents(ThreadedConsumers);
            monitor.Start();
        }

        private static StringBuilder PayOrders(Cashier cashier, Dictionary<string, DateTime> startTimes, int numberOfOrders)
        {
            var ordersPaid = 0;
            var buffer = new StringBuilder();
            do
            {
                var ordersToPay = cashier.GetOrdersReadyToPay();
                foreach (var order in ordersToPay)
                {
                    cashier.PayBill(order);
                    var startTime = startTimes[order.Id];
                    buffer.AppendFormat("Order paid. Time: {0} Id {1} {2}", (DateTime.Now - startTime), order.Id,
                                        Environment.NewLine);
                    ordersPaid++;
                }
            } while (ordersPaid < numberOfOrders);
            return buffer;
        }

        private static void StartPlacingOrders(int numberOfOrders, Waitress waitress, Dictionary<string, DateTime> startTimes)
        {
            for (var i = 0; i < numberOfOrders; i++)
            {
                var orderId = PlaceNewOrder(waitress, i);
                startTimes.Add(orderId, DateTime.Now);

                Thread.Sleep(100);
            }
        }

        private static string PlaceNewOrder(Waitress waitress, int i)
        {
            var orderId = waitress.PlaceOrder(i, i%2 == 0 ? "good looking" : "dodgy", new[]
                                                                                          {
                                                                                              new Item("Sushi", 2),
                                                                                              new Item("Clean glass", 2),
                                                                                              new Item("Sake", 2),
                                                                                          });
            return orderId;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RestaurantKata.Infrastructure;

namespace RestaurantKata
{
    class Program
    {
        private static readonly IList<IHaveAQueue> MonitoredConsumers = new List<IHaveAQueue>();
        private static readonly IList<IStartable> StartableThings = new List<IStartable>(); 

        static void Main()
        {
#warning TODO: Replace topics with message types???
#warning TODO: Make TimeToLiveHandler work with OrderMessageBase

            var waitress = new Waitress("Sexy Mary");
            var kitchen = PrepareKitchen();
            var assistantManager = PrepareAssistantManager();
            var cashier = new Cashier();
            var threadedCashier = PrepareThreadedCashier(cashier);

            var midgetHouse = new MidgetHouse();
            TopicPubSub.Instance.Subscribe(Topics.NewOrders, midgetHouse);
            TopicPubSub.Instance.Subscribe(Topics.CooksQueue, kitchen);
            TopicPubSub.Instance.Subscribe(Topics.Pricing, assistantManager);
            TopicPubSub.Instance.Subscribe(Topics.Payment, threadedCashier);
            TopicPubSub.Instance.Subscribe(Topics.CompletedOrders, midgetHouse);

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

        private static ThreadedConsumer<OrderReadyForPayment> PrepareThreadedCashier(Cashier cashier)
        {
            var threadedCashier = new ThreadedConsumer<OrderReadyForPayment>("Cashier", cashier);
            MonitoredConsumers.Add(threadedCashier);
            StartableThings.Add(threadedCashier);
            return threadedCashier;
        }

        private static ThreadedConsumer<OrderReadyForPricing> PrepareAssistantManager()
        {
            var assistantManager = new ThreadedConsumer<OrderReadyForPricing>("AssistantManager", new AssistantManager());
            MonitoredConsumers.Add(assistantManager);
            StartableThings.Add(assistantManager);
            return assistantManager;
        }

        private static void Start()
        {
            StartQueueMonitoring();

            foreach (var startable in StartableThings)
            {
                startable.Start();
            }
        }

        private static ThreadedConsumer<OrderReadyToCook> PrepareKitchen()
        {
            var threadedCooks = new List<ThreadedConsumer<OrderReadyToCook>>();
            const int numberOfCooks = 3;
            for (var i = 0; i < numberOfCooks; i++)
            {
                var threadedCook = new ThreadedConsumer<OrderReadyToCook>("Cook", new TimeToLiveHandler(new Cook()));
                threadedCooks.Add(threadedCook);
                MonitoredConsumers.Add(threadedCook);
                StartableThings.Add(threadedCook);
            }

            var dispatcher = new ThreadedConsumer<OrderReadyToCook>("Dispatcher", new OrderDispatcher<OrderReadyToCook>(threadedCooks), int.MaxValue);
            MonitoredConsumers.Add(dispatcher);
            StartableThings.Add(dispatcher);
            return dispatcher;
        }

        private static void StartQueueMonitoring()
        {
            var monitor = new QueueMonitor();

            monitor.AddComponents(MonitoredConsumers);
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

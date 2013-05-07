using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RestaurantKata.Infrastructure;

namespace RestaurantKata
{
    internal class QueueMonitor
    {
        private readonly IList<ThreadedConsumer<IOrderConsumer>> components;

        public QueueMonitor()
        {
            this.components = new List<ThreadedConsumer<IOrderConsumer>>();
        }

        public void AddComponents(IEnumerable<ThreadedConsumer<IOrderConsumer>> orderConsumers)
        {
            foreach (var orderConsumer in orderConsumers)
            {
                AddComponent(orderConsumer);
            }
        }

        public void AddComponent(ThreadedConsumer<IOrderConsumer> orderConsumer)
        {
            components.Add(orderConsumer);
        }

        public void Start()
        {
            Task.Run(() =>
                     {
                         while (true)
                         {
                             foreach (var component in components)
                             {
                                 Console.WriteLine("Queue {0} Count: {1}", component.QueneName, component.CountOfItemsInQueue);
                             }
                             Thread.Sleep(1000);
                         }
                     });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantKata.Infrastructure
{
    public class QueueMonitor
    {
        private readonly IList<ThreadedConsumer<IOrderConsumer>> components;

        public QueueMonitor()
        {
            components = new List<ThreadedConsumer<IOrderConsumer>>();
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
                                 Console.WriteLine("Queue {0} Count: {1}", component.Name, component.CountOfItemsInQueue);
                             }
                             Thread.Sleep(1000);
                         }
                     });
        }
    }
}
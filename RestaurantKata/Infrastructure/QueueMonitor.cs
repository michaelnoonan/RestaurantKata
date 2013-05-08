using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantKata.Infrastructure
{
    public class QueueMonitor
    {
        private readonly IList<IHaveAQueue> components;

        public QueueMonitor()
        {
            components = new List<IHaveAQueue>();
        }

        public void AddComponents(IEnumerable<IHaveAQueue> thingsWithAQueue)
        {
            foreach (var thingWithQueue in thingsWithAQueue)
            {
                AddComponent(thingWithQueue);
            }
        }

        public void AddComponent(IHaveAQueue thingWithQueue)
        {
            components.Add(thingWithQueue);
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
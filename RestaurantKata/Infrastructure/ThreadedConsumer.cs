using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantKata.Infrastructure
{
    public class ThreadedConsumer<T> : IOrderConsumer, IStartable where T : IOrderConsumer
    {
        public T Consumer { get { return consumer; } }
        public string QueneName 
        {
            get { return consumer.GetType().Name; }
        }

        public int CountOfItemsInQueue {
            get
            {
                return ordersToProcess.Count;
            }
        }

        private readonly T consumer;
        private readonly ConcurrentQueue<Order> ordersToProcess = new ConcurrentQueue<Order>();

        public ThreadedConsumer(T consumer)
        {
            this.consumer = consumer;
        }

        public void Start()
        {
            Task.Run((Action)DispatchOrders);
        }

        private void DispatchOrders()
        {
            while (true)
            {
                Order order;
                if (ordersToProcess.TryDequeue(out order))
                {
                    consumer.Consume(order);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        public void Consume(Order order)
        {
            ordersToProcess.Enqueue(order);
        }
    }
}
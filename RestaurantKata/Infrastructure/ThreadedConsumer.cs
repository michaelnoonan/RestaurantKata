using System;
using System.Collections.Concurrent;
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
        private readonly int queueLimit;
        private readonly ConcurrentQueue<Order> ordersToProcess = new ConcurrentQueue<Order>();

        public ThreadedConsumer(T consumer, int queueLimit = 10)
        {
            this.consumer = consumer;
            this.queueLimit = queueLimit;
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

        public bool Consume(Order order)
        {
            if (ordersToProcess.Count >= queueLimit) return false;
            ordersToProcess.Enqueue(order);
            return true;
        }
    }
}
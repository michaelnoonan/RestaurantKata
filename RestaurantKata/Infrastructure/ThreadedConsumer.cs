using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantKata.Infrastructure
{
    public class ThreadedConsumer<T> : IOrderConsumer, IStartable
        where T : IOrderConsumer
    {
        public T Consumer { get { return _consumer; } }
        private readonly T _consumer;
        private readonly ConcurrentQueue<Order> _ordersToProcess = new ConcurrentQueue<Order>();

        public ThreadedConsumer(T consumer)
        {
            _consumer = consumer;
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
                if (_ordersToProcess.TryDequeue(out order))
                {
                    _consumer.Consume(order);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        public void Consume(Order order)
        {
            _ordersToProcess.Enqueue(order);
        }
    }
}
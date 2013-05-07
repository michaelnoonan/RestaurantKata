using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RestaurantKata.Infrastructure
{
    public class RoundRobinConsumer : IOrderConsumer
    {
        private readonly ConcurrentQueue<IOrderConsumer> _consumers;
        public RoundRobinConsumer(IEnumerable<IOrderConsumer> consumers)
        {
            _consumers = new ConcurrentQueue<IOrderConsumer>(consumers);
        }

        public void Consume(Order order)
        {
            IOrderConsumer nextConsumer;
            
            if (_consumers.TryDequeue(out nextConsumer))
            {
                nextConsumer.Consume(order);
                _consumers.Enqueue(nextConsumer);
            }
            else
            {
                throw new Exception("Ummmmmm, this should never happen.");
            }
        }
    }
}
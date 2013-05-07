using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace RestaurantKata.Infrastructure
{
    public class OrderDispatcher : IOrderConsumer
    {
        private readonly ConcurrentQueue<IOrderConsumer> consumers;
        public OrderDispatcher(IEnumerable<IOrderConsumer> consumers)
        {
            this.consumers = new ConcurrentQueue<IOrderConsumer>(consumers);
        }

        public bool Consume(Order order)
        {
            bool consumed;
            do
            {
                IOrderConsumer nextConsumer;
                if (consumers.TryDequeue(out nextConsumer))
                {
                    consumers.Enqueue(nextConsumer);
                    consumed = nextConsumer.Consume(order);
                    if (!consumed) Thread.Sleep(100);                 
                }
                else
                {
                    throw new Exception("Ummmmmm, this should never happen.");
                }
            } while (!consumed);

            return true;
        }
    }
}
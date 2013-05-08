using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace RestaurantKata.Infrastructure
{
    public class OrderDispatcher<TMessage> : IConsume<TMessage>
        where TMessage : IMessage
    {
        private readonly ConcurrentQueue<IConsume<TMessage>> consumers;
        public OrderDispatcher(IEnumerable<IConsume<TMessage>> consumers)
        {
            this.consumers = new ConcurrentQueue<IConsume<TMessage>>(consumers);
        }

        public bool Consume(TMessage message)
        {
            bool consumed;
            do
            {
                IConsume<TMessage> nextConsumer;
                if (consumers.TryDequeue(out nextConsumer))
                {
                    consumers.Enqueue(nextConsumer);
                    consumed = nextConsumer.Consume(message);
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
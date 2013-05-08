using System;
using System.Collections.Concurrent;

namespace RestaurantKata.Infrastructure
{
    public class MultiPlexer : IConsume<IMessage>
    {
        private readonly ConcurrentQueue<object> consumers;

        public MultiPlexer(params object[] consumers)
        {
            this.consumers = new ConcurrentQueue<object>(consumers);
        }

        public bool Consume(IMessage message)
        {
            foreach (var consumer in consumers)
            {
#warning Yeah, we should probably do this betterer
                dynamic rockOnAndDoWhatISay = consumer;
                rockOnAndDoWhatISay.Consume((dynamic)message);
            }

            return true;
        }

        public void Add(object consumer)
        {
            consumers.Enqueue(consumer);
        }
    }
}
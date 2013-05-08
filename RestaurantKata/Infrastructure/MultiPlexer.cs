using System.Collections.Concurrent;

namespace RestaurantKata.Infrastructure
{
    public class MultiPlexer : IConsume<IMessage>
    {
        private readonly ConcurrentQueue<IConsume<IMessage>> consumers;

        public MultiPlexer(params IConsume<IMessage>[] consumers)
        {
            this.consumers = new ConcurrentQueue<IConsume<IMessage>>(consumers);
        }

        public bool Consume(IMessage message)
        {
            foreach (var consumer in consumers)
            {
                consumer.Consume(message);
            }

            return true;
        }

        public void Add(IConsume<IMessage> consumer)
        {
            consumers.Enqueue(consumer);
        }
    }
}
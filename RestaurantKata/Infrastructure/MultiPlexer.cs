using System.Collections.Concurrent;

namespace RestaurantKata.Infrastructure
{
    public class MultiPlexer : IOrderConsumer
    {
        private readonly ConcurrentQueue<IOrderConsumer> consumers;

        public MultiPlexer(params IOrderConsumer[] consumers)
        {
            this.consumers = new ConcurrentQueue<IOrderConsumer>(consumers);
        }

        public bool Consume(Order order)
        {
            foreach (var consumer in consumers)
            {
                consumer.Consume(order);
            }

            return true;
        }

        public void Add(IOrderConsumer consumer)
        {
            consumers.Enqueue(consumer);
        }
    }
}
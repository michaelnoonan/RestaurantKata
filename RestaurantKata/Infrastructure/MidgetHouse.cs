using System.Collections.Concurrent;

namespace RestaurantKata.Infrastructure
{
    public class MidgetHouse : IOrderConsumer
    {
        private readonly ConcurrentDictionary<string, Midget> house = new ConcurrentDictionary<string, Midget>();

        public bool Consume(Order order)
        {
            if (order.IsNew())
            {
                var midget = house.GetOrAdd(order.CorrelationId, key => new Midget(key));
                TopicPubSub.Instance.Subscribe(order.CorrelationId, midget);
                TopicPubSub.Instance.Publish(order.CorrelationId, order);
                return true;
            }

            Midget unused;
            house.TryRemove(order.CorrelationId, out unused);
            return true;
        }
    }
}
using System.Collections.Concurrent;

namespace RestaurantKata.Infrastructure
{
    public class MidgetHouse :
        IConsume<OrderPlaced>,
        IConsume<OrderCompleted>
    {
        private readonly ConcurrentDictionary<string, Midget> house = new ConcurrentDictionary<string, Midget>();

        public bool Consume(OrderPlaced message)
        {
            var midget = house.GetOrAdd(message.Order.CorrelationId, key => new Midget(key));
            TopicPubSub.Instance.Subscribe(message.Order.CorrelationId, midget);
            midget.StartOrder(message.Order);
            return true;
        }

        public bool Consume(OrderCompleted message)
        {
            Midget unused;
            house.TryRemove(message.Order.CorrelationId, out unused);
            return true;
        }
    }
}
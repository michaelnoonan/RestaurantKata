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
            TopicPubSub.Instance.Subscribe<FoodCooked>(message.Order.CorrelationId, midget);
            TopicPubSub.Instance.Subscribe<OrderPriced>(message.Order.CorrelationId, midget);
            TopicPubSub.Instance.Subscribe<OrderPaid>(message.Order.CorrelationId, midget);
            TopicPubSub.Instance.Subscribe<OrderDropped>(message.Order.CorrelationId, midget);
            //TODO: Make it work with reflection
            //TopicPubSub.Instance.SubscribeAll(message.Order.CorrelationId, midget);
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
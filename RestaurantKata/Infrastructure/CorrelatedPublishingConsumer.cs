namespace RestaurantKata.Infrastructure
{
    public class CorrelatedPublishingConsumer : IOrderConsumer
    {
        public bool Consume(Order order)
        {
            TopicPubSub.Instance.Publish(order.CorrelationId, order);
            return true;
        }
    }
}
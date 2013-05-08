namespace RestaurantKata.Infrastructure
{
    public class Midget : IOrderConsumer
    {
        public string CorrelationId { get; private set; }

        public Midget(string correlationId)
        {
            CorrelationId = correlationId;
        }

        public bool Consume(Order order)
        {
            if (!order.IsCooked())
            {
                TopicPubSub.Instance.Publish(Topics.CooksQueue, order);
                return true;
            }

            if (!order.IsPriced())
            {
                TopicPubSub.Instance.Publish(Topics.Pricing, order);
                return true;
            }

            if (!order.IsPaid())
            {
                TopicPubSub.Instance.Publish(Topics.Payment, order);
                return true;
            }

            TopicPubSub.Instance.Publish(Topics.CompletedOrders, order);
            return true;
        }
    }
}
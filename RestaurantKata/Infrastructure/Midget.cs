namespace RestaurantKata.Infrastructure
{
    public class Midget :
        IConsume<FoodCooked>,
        IConsume<OrderPriced>,
        IConsume<OrderPaid>
    {
        public string CorrelationId { get; private set; }

        public Midget(string correlationId)
        {
            CorrelationId = correlationId;
        }

        public void StartOrder(Order order)
        {
            TopicPubSub.Instance.Publish(Topics.CooksQueue, new OrderReadyToCook { Order = order});
        }

        public bool Consume(FoodCooked message)
        {
            TopicPubSub.Instance.Publish(Topics.Pricing, new OrderReadyForPricing { Order = message.Order });
            return true;
        }

        public bool Consume(OrderPriced message)
        {
            TopicPubSub.Instance.Publish(Topics.Payment, new OrderReadyForPayment { Order = message.Order });
            return true;
        }

        public bool Consume(OrderPaid message)
        {
            TopicPubSub.Instance.Publish(Topics.CompletedOrders, new OrderCompleted { Order = message.Order });
            return true;
        }
    }
}
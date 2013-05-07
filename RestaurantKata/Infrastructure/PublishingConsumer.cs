namespace RestaurantKata.Infrastructure
{
    public class PublishingConsumer : IOrderConsumer
    {
        private readonly string topic;

        public PublishingConsumer(string topic)
        {
            this.topic = topic;
        }

        public bool Consume(Order order)
        {
            TopicPubSub.Instance.Publish(topic, order);
            return true;
        }
    }
}
namespace RestaurantKata.Infrastructure
{
    public class NarrowingConsumer<TMessage> : IConsume<IMessage> where TMessage : IMessage
    {
        private readonly IConsume<TMessage> consumer;

        public NarrowingConsumer(IConsume<TMessage> consumer)
        {
            this.consumer = consumer;
        }

        public bool Consume(IMessage message)
        {
            //TODO: Hack, find a better way
            if (message.GetType() == typeof (TMessage))
            {
                return consumer.Consume((TMessage) message);
            }
            return true;
        }
    }
}
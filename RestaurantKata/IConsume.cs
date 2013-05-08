namespace RestaurantKata
{
    public interface IConsume<in TMessage> where TMessage : IMessage
    {
        bool Consume(TMessage message);
    }
}
namespace RestaurantKata
{
    public interface IOrderConsumer
    {
        bool Consume(Order order);
    }
}
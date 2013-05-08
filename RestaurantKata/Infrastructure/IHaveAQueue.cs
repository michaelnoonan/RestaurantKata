namespace RestaurantKata.Infrastructure
{
    public interface IHaveAQueue
    {
        string Name { get; }
        int CountOfItemsInQueue { get; }
    }
}
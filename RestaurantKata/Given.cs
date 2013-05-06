namespace RestaurantKata
{
    public class Given
    {
        public static Order AnOrderForJapanese(string server = "Sexy Mary", int tableNumber = 15, string customerCategory = "dodgy", Item[] items = null)
        {
            if (items == null)
            {
                items = new[]
                            {
                                new Item("Sushi", 2),
                                new Item("Clean glass", 2),
                                new Item("Sake", 2),
                            };
            }

            return new Order
                       {
                           Server = server,
                           TableNumber = tableNumber,
                           CustomerCategory = customerCategory,
                           Items = items
                       };
        }
    }
}
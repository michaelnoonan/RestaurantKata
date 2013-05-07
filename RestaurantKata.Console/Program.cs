using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RestaurantKata
{
    class Program
    {
        static void Main(string[] args)
        {
            var cashier = new Cashier(new ConsoleOrderConsumerProcessor());
            var assistantManager = new AssistantManager(cashier);
            var cook = new Cook(assistantManager);
            var waitress = new Waitress("Sexy Mary", cook);

            for (int i = 0; i < 2; i++)
            {
                waitress.PlaceOrder(i, i % 2 == 0 ? "good looking" : "dodgy", new[]
                                                     {
                                                         new Item("Sushi", 2),
                                                         new Item("Clean glass", 2),
                                                         new Item("Sake", 2),
                                                     });
                cashier.PayBill(i, 100M);
            }

            Console.ReadLine();
        }
    }

    public class ConsoleOrderConsumerProcessor : IOrderConsumer
    {
        public void Consume(Order order)
        {
            var settings = new JsonSerializerSettings
                               {
                                   Formatting = Formatting.Indented,
                                   ContractResolver = new CamelCasePropertyNamesContractResolver()
                               };
            
            Console.WriteLine(JsonConvert.SerializeObject(order, settings));
        }
    }

}

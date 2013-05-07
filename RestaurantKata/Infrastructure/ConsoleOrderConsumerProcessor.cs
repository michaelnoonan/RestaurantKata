using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RestaurantKata.Infrastructure
{
    public class ConsoleOrderConsumerProcessor : IOrderConsumer
    {
        public bool Consume(Order order)
        {
            var settings = new JsonSerializerSettings
                           {
                               Formatting = Formatting.Indented,
                               ContractResolver = new CamelCasePropertyNamesContractResolver()
                           };

            return true;

            //Console.WriteLine(JsonConvert.SerializeObject(order, settings));
        }
    }
}
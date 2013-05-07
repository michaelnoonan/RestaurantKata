using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RestaurantKata
{
    public class ConsoleOrderConsumerProcessor : IOrderConsumer
    {
        public void Consume(Order order)
        {
            var settings = new JsonSerializerSettings
                           {
                               Formatting = Formatting.Indented,
                               ContractResolver = new CamelCasePropertyNamesContractResolver()
                           };
            
            //Console.WriteLine(JsonConvert.SerializeObject(order, settings));
        }
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RestaurantKata.Infrastructure
{
    public class ConsoleConsumeProcessor<TMessage> : IConsume<TMessage>
        where TMessage : IMessage
    {
        public bool Consume(TMessage message)
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestaurantKata
{
    class Program
    {
        static void Main(string[] args)
        {
            var waitress = new Waitress("Sexy Mary", new ConsoleOrderProcessor());

            waitress.PlaceOrder(15, "dodgy", new[]
                                                 {
                                                     new Item("Sushi", 2),
                                                     new Item("Clean glass", 2),
                                                     new Item("Sake", 2),
                                                 });

            Console.ReadLine();

        }
    }

    public class ConsoleOrderProcessor : IProcessOrder
    {
        public void Process(Order order)
        {
            Console.WriteLine(JsonConvert.SerializeObject(order, Formatting.Indented));
        }
    }

}

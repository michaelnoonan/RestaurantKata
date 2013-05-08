using System;
using NSubstitute;
using NUnit.Framework;
using RestaurantKata.Infrastructure;

namespace RestaurantKata
{
    public class Waitress
    {
        public string Name { get; protected set; }

        public Waitress(string name)
        {
            Name = name;
        }

        public string PlaceOrder(int tableNumber, string customerCategory, Item[] items)
        {
            var order = new Order
                            {
                                Id = Guid.NewGuid().ToString(),
                                Server = Name,
                                TableNumber = tableNumber,
                                CustomerCategory = customerCategory,
                                Items = items,
                                TimeToLive =  DateTime.Now.AddSeconds(10)
                            };

            TopicPubSub.Instance.Publish(Topics.NewOrders, new OrderPlaced { Order = order });
            return order.Id;
        }
    }

    //[TestFixture]
    //public class WaitressTests
    //{
    //    [Test]
    //    public void TheWaitressCanTakeAnOrder()
    //    {
    //        var waitress = new Waitress("Sexy Mary");
            
    //        waitress.PlaceOrder(15, "dodgy", new[]
    //                                             {
    //                                                 new Item("Sushi", 2),
    //                                                 new Item("Clean glass", 2),
    //                                                 new Item("Sake", 2),
    //                                             });

    //    }
    //}
}
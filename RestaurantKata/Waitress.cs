using System;
using NSubstitute;
using NUnit.Framework;

namespace RestaurantKata
{
    public class Waitress
    {
        private readonly IOrderConsumer nextStep;
        public string Name { get; protected set; }

        public Waitress(string name, IOrderConsumer nextStep)
        {
            this.nextStep = nextStep;
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
                                Items = items
                            };

            nextStep.Consume(order);

            return order.Id;
        }
    }

    [TestFixture]
    public class WaitressTests
    {
        [Test]
        public void TheWaitressCanTakeAnOrder()
        {
            var nextStep = Substitute.For<IOrderConsumer>(); 
            var waitress = new Waitress("Sexy Mary", nextStep);
            
            waitress.PlaceOrder(15, "dodgy", new[]
                                                 {
                                                     new Item("Sushi", 2),
                                                     new Item("Clean glass", 2),
                                                     new Item("Sake", 2),
                                                 });

            nextStep.Received(1).Consume(Arg.Any<Order>());
        }
    }
}
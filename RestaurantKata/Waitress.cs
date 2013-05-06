using System;
using NSubstitute;
using NUnit.Framework;

namespace RestaurantKata
{
    public class Waitress
    {
        private readonly IProcessOrder _nextStep;
        public string Name { get; protected set; }

        public Waitress(string name, IProcessOrder nextStep)
        {
            _nextStep = nextStep;
            Name = name;
        }

        public void PlaceOrder(int tableNumber, string customerCategory, Item[] items)
        {
            var order = new Order
                            {
                                Server = Name,
                                TableNumber = tableNumber,
                                CustomerCategory = customerCategory,
                                Items = items
                            };

            _nextStep.Process(order);
        }
    }

    [TestFixture]
    public class WaitressTests
    {
        [Test]
        public void TheWaitressCanTakeAnOrder()
        {
            var nextStep = Substitute.For<IProcessOrder>(); 
            var waitress = new Waitress("Sexy Mary", nextStep);
            
            waitress.PlaceOrder(15, "dodgy", new[]
                                                 {
                                                     new Item("Sushi", 2),
                                                     new Item("Clean glass", 2),
                                                     new Item("Sake", 2),
                                                 });

            nextStep.Received(1).Process(Arg.Any<Order>());
        }
    }
}
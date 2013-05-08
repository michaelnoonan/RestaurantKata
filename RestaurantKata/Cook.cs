using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using RestaurantKata.Infrastructure;

namespace RestaurantKata
{
    public class Cook : IConsume<OrderReadyToCook>
    {
        private readonly int cookingDelay;
        private static readonly Random random = new Random();

        private readonly Dictionary<string, string[]> recipes =
            new Dictionary<string, string[]>()
                {
                    { "Sake", new[] { "Bottle of sake" } },
                    { "Sushi", new[] { "Sushi Rice", "Vinegar", "Sugar", "Salt", "Salmon", "Nori", "Wasabi", "Imitation roe" } },
                    { "Clean glass", new[] { "Clean glass" } },
                };

        public Cook()
        {
            Name = Guid.NewGuid().ToString();
            cookingDelay = random.Next(100, 2000);
            Console.WriteLine("Cooking delay {0}: {1}", Name, cookingDelay);
        }

        public string Name { get; set; }

        private void PrepareFood(Order order)
        {
            order.CookTime = cookingDelay;
            Thread.Sleep(cookingDelay);
            Console.WriteLine("Food prepared by: " + Name);
            foreach (var item in order.Items)
            {
                item.Ingredients = recipes[item.ItemDescription];
            }
        }

        public bool Consume(OrderReadyToCook message)
        {
            PrepareFood(message.Order);
            TopicPubSub.Instance.Publish(message.CorrelationId, new FoodCooked { Order = message.Order });
            return true;
        }
    }

    //[TestFixture]
    //public class CookTests
    //{
    //    [Test]
    //    public void GivenAnOrder_TheNextStepShouldBeCalled()
    //    {
    //        var nextStep = Substitute.For<IConsume>();
    //        var cook = new Cook(nextStep);
    //        var order = Given.AnOrderForJapanese();
            
    //        cook.Consume(order);

    //        nextStep.Received(1).Consume(order);
    //    }

    //    [Test]
    //    public void GivenAnOrder_TheOrderShouldBeEnrichedWithIngredients()
    //    {
    //        var nextStep = Substitute.For<IConsume>();
    //        var cook = new Cook(nextStep);
    //        var order = Given.AnOrderForJapanese();
            
    //        cook.Consume(order);

    //        Assert.That(order.Items.All(x => x.Ingredients.Any()));
    //    }
    //}
}
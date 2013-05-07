using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;

namespace RestaurantKata
{
    public class Cook : IOrderConsumer
    {
        private readonly IOrderConsumer nextStep;
        private readonly int cookingDelay;
        private static readonly Random Random = new Random();

        private readonly Dictionary<string, string[]> recipes =
            new Dictionary<string, string[]>()
                {
                    { "Sake", new[] { "Bottle of sake" } },
                    { "Sushi", new[] { "Sushi Rice", "Vinegar", "Sugar", "Salt", "Salmon", "Nori", "Wasabi", "Imitation roe" } },
                    { "Clean glass", new[] { "Clean glass" } },
                };

        public Cook(IOrderConsumer nextStep)
        {
            Name = Guid.NewGuid().ToString();
            this.nextStep = nextStep;
            cookingDelay = new Random().Next(100, 2000);
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

        public bool Consume(Order order)
        {
            PrepareFood(order);
            return nextStep.Consume(order);
        }
    }

    [TestFixture]
    public class CookTests
    {
        [Test]
        public void GivenAnOrder_TheNextStepShouldBeCalled()
        {
            var nextStep = Substitute.For<IOrderConsumer>();
            var cook = new Cook(nextStep);
            var order = Given.AnOrderForJapanese();
            
            cook.Consume(order);

            nextStep.Received(1).Consume(order);
        }

        [Test]
        public void GivenAnOrder_TheOrderShouldBeEnrichedWithIngredients()
        {
            var nextStep = Substitute.For<IOrderConsumer>();
            var cook = new Cook(nextStep);
            var order = Given.AnOrderForJapanese();
            
            cook.Consume(order);

            Assert.That(order.Items.All(x => x.Ingredients.Any()));
        }
    }
}
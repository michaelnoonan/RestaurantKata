using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;

namespace RestaurantKata
{
    public class Cook : IOrderConsumer
    {
        private readonly IOrderConsumer _nextStep;

        private readonly Dictionary<string, string[]> _recipes =
            new Dictionary<string, string[]>()
                {
                    { "Sake", new[] { "Bottle of sake" } },
                    { "Sushi", new[] { "Sushi Rice", "Vinegar", "Sugar", "Salt", "Salmon", "Nori", "Wasabi", "Imitation roe" } },
                    { "Clean glass", new[] { "Clean glass" } },
                };

        public Cook(IOrderConsumer nextStep)
        {
            _nextStep = nextStep;
        }

        private void PrepareFood(Order order)
        {
            Thread.Sleep(500);
            order.Cooktime = 500;
            foreach (var item in order.Items)
            {
                item.Ingredients = _recipes[item.ItemDescription];
            }
        }

        public void Consume(Order order)
        {
            PrepareFood(order);
            _nextStep.Consume(order);
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
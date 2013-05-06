using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;

namespace RestaurantKata
{
    public class AssistantManager : IOrderConsumer
    {
        private readonly IDictionary<string, decimal> priceList =
            new Dictionary<string, decimal>
                {
                    { "Sake", 15M },
                    { "Sushi", 10M },
                    { "Clean glass", 50M },
                };

        private readonly IOrderConsumer _nextStep;

        public AssistantManager(IOrderConsumer nextStep)
        {
            _nextStep = nextStep;
        }

        public void Consume(Order order)
        {
            PriceOrder(order);
            _nextStep.Consume(order);
        }

        private void PriceOrder(Order order)
        {
            Thread.Sleep(500);
            foreach (var item in order.Items)
            {
                item.Price = priceList[item.ItemDescription];
            }
        }
    }

    [TestFixture]
    public class AssistantManagerTests
    {
        [Test]
        public void GivenAnOrder_CallsTheNextStep()
        {
            var nextStep = Substitute.For<IOrderConsumer>();
            var assistantManager = new AssistantManager(nextStep);
            var order = Given.AnOrderForJapanese();

            assistantManager.Consume(order);

            nextStep.Received(1).Consume(order);
        }

        [Test]
        public void GivenAnOrder_EnrichesOrderWithPrices()
        {
            var nextStep = Substitute.For<IOrderConsumer>();
            var assistantManager = new AssistantManager(nextStep);
            var order = Given.AnOrderForJapanese();

            assistantManager.Consume(order);

            Assert.That(order.Items.All(x => x.Price.HasValue));
        }
    }
}
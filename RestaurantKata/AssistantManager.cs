using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using RestaurantKata.Infrastructure;

namespace RestaurantKata
{
    public class AssistantManager : IConsume<OrderReadyForPricing>
    {
        private readonly IDictionary<string, decimal> priceList =
            new Dictionary<string, decimal>
                {
                    { "Sake", 15M },
                    { "Sushi", 10M },
                    { "Clean glass", 50M },
                };

        public bool Consume(OrderReadyForPricing message)
        {
            PriceOrder(message.Order);
            TopicPubSub.Instance.Publish(message.CorrelationId, new OrderPriced { Order = message.Order });
            return true;
        }

        private void PriceOrder(Order order)
        {
            foreach (var item in order.Items)
            {
                item.Price = priceList[item.ItemDescription];
            }
        }
    }

    //[TestFixture]
    //public class AssistantManagerTests
    //{
    //    [Test]
    //    public void GivenAnOrder_CallsTheNextStep()
    //    {
    //        var nextStep = Substitute.For<IConsume>();
    //        var assistantManager = new AssistantManager(nextStep);
    //        var order = Given.AnOrderForJapanese();

    //        assistantManager.Consume(order);

    //        nextStep.Received(1).Consume(order);
    //    }

    //    [Test]
    //    public void GivenAnOrder_EnrichesOrderWithPrices()
    //    {
    //        var nextStep = Substitute.For<IConsume>();
    //        var assistantManager = new AssistantManager(nextStep);
    //        var order = Given.AnOrderForJapanese();

    //        assistantManager.Consume(order);

    //        Assert.That(order.Items.All(x => x.Price.HasValue));
    //    }
    //}
}
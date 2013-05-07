using System.Collections.Concurrent;
using NSubstitute;
using NUnit.Framework;

namespace RestaurantKata.Infrastructure
{
    public class TopicPubSub
    {
        static TopicPubSub()
        {
            Instance = new TopicPubSub();
        }

        readonly ConcurrentDictionary<string, MultiPlexer> subscriptions = new ConcurrentDictionary<string, MultiPlexer>();

        public void Subscribe(string topic, IOrderConsumer consumer)
        {
            subscriptions.AddOrUpdate(topic, key => new MultiPlexer(consumer), 
                                      (key, multiPlexer) =>
                                          {
                                              multiPlexer.Add(consumer);
                                              return multiPlexer;
                                          }); 
        }

        public void Publish(string topic, Order order)
        {
            var subscribers = subscriptions[topic];
            subscribers.Consume(order);
        }

        public static TopicPubSub Instance
        {
            get; private set;
        }
    }

    [TestFixture]
    public class Tests
    {
        [Test]
        public void PublisSubscribeWorks()
        {
            var topicPubSub = new TopicPubSub();
            var subscriber = Substitute.For<IOrderConsumer>();
            topicPubSub.Subscribe("Test", subscriber);
            topicPubSub.Publish("Test", new Order());

            subscriber.Received(1).Consume(Arg.Any<Order>());
        }
    }
}
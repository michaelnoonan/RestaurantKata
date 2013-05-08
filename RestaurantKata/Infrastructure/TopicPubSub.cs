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

        public void Subscribe(string topic, object consumer)
        {
            subscriptions.AddOrUpdate(topic, key => new MultiPlexer(consumer), 
                                      (key, multiPlexer) =>
                                          {
                                              multiPlexer.Add(consumer);
                                              return multiPlexer;
                                          }); 
        }

        public void Publish(string topic, IEvent @event)
        {
            var multiplexer = subscriptions[topic];
            multiplexer.Consume(@event);
        }

        public static TopicPubSub Instance
        {
            get; private set;
        }
    }

    //[TestFixture]
    //public class Tests
    //{
    //    [Test]
    //    public void PublisSubscribeWorks()
    //    {
    //        var topicPubSub = new TopicPubSub();
    //        var subscriber = Substitute.For<IConsume>();
    //        topicPubSub.Subscribe("Test", subscriber);
    //        topicPubSub.Publish("Test", new Order());

    //        subscriber.Received(1).Consume(Arg.Any<Order>());
    //    }
    //}
}
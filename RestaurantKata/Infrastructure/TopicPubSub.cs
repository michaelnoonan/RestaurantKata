using System.Collections.Concurrent;
using System.Linq;
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


        public void Subscribe<TMessage>(string topic, IConsume<TMessage> consumer) where TMessage : IMessage
        {
            subscriptions.AddOrUpdate(topic, key => new MultiPlexer(new NarrowingConsumer<TMessage>(consumer)), 
                                      (key, multiPlexer) =>
                                          {
                                              multiPlexer.Add(new NarrowingConsumer<TMessage>(consumer));
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

        public void SubscribeAll(string correlationId, object subscriber)
        {
            var iConsume = subscriber.GetType().GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsume<>)).ToList();

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
using System;

namespace RestaurantKata.Infrastructure
{
    public class TimeToLiveHandler : IConsume<OrderReadyToCook>
    {
        private readonly IConsume<OrderReadyToCook> nextStep;

        public TimeToLiveHandler(IConsume<OrderReadyToCook> nextStep)
        {
            this.nextStep = nextStep;
        }

        public bool Consume(OrderReadyToCook message)
        {
            if (message.Order.TimeToLive < DateTime.Now)
            {
                Logger.Error("Order dropped. Id {0}", message.Order.Id);
                TopicPubSub.Instance.Publish(message.CorrelationId, new OrderDropped(){Order =  message.Order});
            }
            else
            {
                return nextStep.Consume(message);
            }

            return true;
        }
    }
}
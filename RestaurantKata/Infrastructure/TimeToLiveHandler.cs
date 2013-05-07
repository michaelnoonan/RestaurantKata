using System;

namespace RestaurantKata.Infrastructure
{
    public class TimeToLiveHandler : IOrderConsumer
    {
        private readonly IOrderConsumer nextStep;

        public TimeToLiveHandler(IOrderConsumer nextStep)
        {
            this.nextStep = nextStep;
        }

        public bool Consume(Order order)
        {
            if (order.TimeToLive < DateTime.Now)
            {
                Console.WriteLine("Order dropped. Id {0}", order.Id);
            }
            else
            {
                return nextStep.Consume(order);
            }

            return true;
        }
    }
}
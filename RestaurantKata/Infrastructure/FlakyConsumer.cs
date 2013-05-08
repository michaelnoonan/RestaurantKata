using System;

namespace RestaurantKata.Infrastructure
{
    public class FlakyConsumer<TMessage> : IConsume<TMessage> where TMessage : IMessage
    {
        private readonly IConsume<TMessage> nextStep;
        private readonly double chanceToFail;
        private static Random random = new Random();

        public FlakyConsumer(IConsume<TMessage> nextStep, double chanceToFail = 0.1)
        {
            this.nextStep = nextStep;
            this.chanceToFail = chanceToFail;
        }

        public bool Consume(TMessage message)
        {
            var value = random.NextDouble();
            if (value <= chanceToFail) return true;
            return nextStep.Consume(message);
        }
    }
}
namespace RestaurantKata.Infrastructure.Alarm
{
    public class AlarmClock : IConsume<WakeUpCall>
    {
        private readonly IDateTimeComparer comparer;


        public AlarmClock(IDateTimeComparer comparer)
        {
            this.comparer = comparer;
        }

        public bool Consume(WakeUpCall message)
        {
            if (comparer.IsInTheFuture(message.DateTime)) return false;

            message.Callback();
            return true;
        }
    }
}
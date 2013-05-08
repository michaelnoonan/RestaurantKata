using System;

namespace RestaurantKata.Infrastructure.Alarm
{
    public class WakeUpCall : IEvent
    {
        public WakeUpCall(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        public DateTime DateTime { get; set; }
        public string Topic { get; set; }
        public string CorrelationId { get; private set; }

        public CheckeFoodIsCooked MessageForWakeUpCall { get; set; }
    }
}
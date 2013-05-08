using System;

namespace RestaurantKata.Infrastructure.Alarm
{
    public class WakeUpCall : IEvent
    {
        public WakeUpCall(DateTime dateTime, Action callback)
        {
            DateTime = dateTime;
            Callback = callback;
        }

        public DateTime DateTime { get; set; }
        public Action Callback { get; set; }
        public string CorrelationId { get; private set; }
    }
}
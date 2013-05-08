using System;

namespace RestaurantKata.Infrastructure.Alarm
{
    public class WakeUpCall
    {
        public WakeUpCall(DateTime dateTime, Func<bool> callback)
        {
            DateTime = dateTime;
            Callback = callback;
        }

        public DateTime DateTime { get; set; }
        public Func<bool> Callback { get; set; }
    }
}
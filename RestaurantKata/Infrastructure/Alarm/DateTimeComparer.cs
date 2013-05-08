using System;

namespace RestaurantKata.Infrastructure.Alarm
{
    public class DateTimeComparer : IDateTimeComparer
    {
        public bool IsInTheFuture(DateTime candidate)
        {
            return DateTime.Now < candidate;
        }
    }
}
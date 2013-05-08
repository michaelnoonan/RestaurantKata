using System;

namespace RestaurantKata.Infrastructure.Alarm
{
    public interface IDateTimeComparer
    {
        bool IsInTheFuture(DateTime candidate);
    }
}
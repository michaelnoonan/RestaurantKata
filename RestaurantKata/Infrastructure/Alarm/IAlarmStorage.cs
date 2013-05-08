namespace RestaurantKata.Infrastructure.Alarm
{
    public interface IAlarmStorage
    {
        void Enqueue(WakeUpCall wakeUpCall);
        WakeUpCall Dequeue();
    }
}
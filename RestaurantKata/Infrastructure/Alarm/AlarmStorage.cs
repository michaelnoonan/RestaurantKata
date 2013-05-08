using System.Collections.Generic;

namespace RestaurantKata.Infrastructure.Alarm
{
    public class AlarmStorage : IAlarmStorage
    {
        public AlarmStorage()
        {
            Queue = new Queue<WakeUpCall>();
        }

        private Queue<WakeUpCall> Queue { get; set; }

        public void Enqueue(WakeUpCall wakeUpCall)
        {
            Queue.Enqueue(wakeUpCall);
        }

        public WakeUpCall Dequeue()
        {
            return Queue.Dequeue();
        }
    }
}
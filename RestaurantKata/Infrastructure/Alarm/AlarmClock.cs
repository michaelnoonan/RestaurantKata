namespace RestaurantKata.Infrastructure.Alarm
{
    public class AlarmClock
    {
        private readonly IAlarmStorage _storage;
        private readonly IDateTimeComparer _comparer;

        public AlarmClock(IAlarmStorage storage, IDateTimeComparer comparer)
        {
            _storage = storage;
            _comparer = comparer;
        }

        public void Start()
        {

        }

        public void Do()
        {
            var wakeUpCall = _storage.Dequeue();
            if (_comparer.IsInTheFuture(wakeUpCall.DateTime))
            {
                _storage.Enqueue(wakeUpCall);
            }
            else
            {
                wakeUpCall.Callback();
            }
        }

        public void SetAlarm(WakeUpCall wakeUpCall)
        {
            _storage.Enqueue(wakeUpCall);
        }
    }
}
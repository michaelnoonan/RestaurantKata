using System;
using NSubstitute;
using NUnit.Framework;

namespace RestaurantKata.Infrastructure.Alarm
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void GivenAnAlarmInthefuture_itshould_queueforlater()
        {
            var called = false;
            var alarmStorage = Substitute.For<IAlarmStorage>();
            alarmStorage.Dequeue().Returns(new WakeUpCall(DateTime.MinValue, () => called = true));
            var dateTimeComparer = Substitute.For<IDateTimeComparer>();
            dateTimeComparer.IsInTheFuture(Arg.Any<DateTime>()).Returns(true);
            var alarm = new AlarmClock(alarmStorage, dateTimeComparer);

            alarm.Do();

            Assert.That(called, Is.False);
        }

        [Test]
        public void GivenAnAlarmInthepast_itshould_queueforlater()
        {
            var called = false;
            var alarmStorage = Substitute.For<IAlarmStorage>();
            alarmStorage.Dequeue().Returns(new WakeUpCall(DateTime.MinValue, () => called = true));
            var dateTimeComparer = Substitute.For<IDateTimeComparer>();
            dateTimeComparer.IsInTheFuture(Arg.Any<DateTime>()).Returns(false);
            var alarm = new AlarmClock(alarmStorage, dateTimeComparer);

            alarm.Do();

            Assert.That(called, Is.True);
        }
    }
}
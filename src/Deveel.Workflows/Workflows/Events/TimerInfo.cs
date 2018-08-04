using System;

namespace Deveel.Workflows.Events
{
    public sealed class TimerInfo
    {
        private TimerInfo(TimerType timerType, DateTimeOffset? absolute, TimeSpan? duration)
        {
            TimerType = timerType;
            Date = absolute ?? DateTimeOffset.MinValue;
            Duration = duration ?? TimeSpan.Zero;
        }

        public TimerInfo(DateTimeOffset date)
            : this(TimerType.Date, date, null)
        {
        }

        public TimerInfo(TimeSpan timeSpan)
            : this(TimerType.Duration, null, timeSpan)
        {
        }

        public TimerType TimerType { get; }

        public DateTimeOffset Date { get; }

        public TimeSpan Duration { get; }

        public bool IsAbsoluteDate => TimerType == TimerType.Date;

        public bool IsTimeSpan => TimerType == TimerType.Duration;
    }
}

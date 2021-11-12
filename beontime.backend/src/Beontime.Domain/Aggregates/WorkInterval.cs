using Beontime.Domain.Events;
using System;

namespace Beontime.Domain.Aggregates
{

    public sealed class WorkInterval
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Guid Id { get; set; }
        public TimeSpan Duration { get; set; }

        public void Apply(GettingWorkStarted start)
        {
            Start = start.Timestamp;
        }
        public void Apply(EndOfWork end)
        {
            End = end.Timestamp;
        }
    }
}

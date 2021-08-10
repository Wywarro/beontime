namespace Beontime.Domain.Aggregates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Enums;

    public class TimeCard
    {
        public TimeCardStatus Status { get; set; }
        public DateTime Day { get; set; }
        public TimeSpan WorkDuration { get; set; }
        public TimeSpan BreakDuration { get; set; }
        public IEnumerable<AttendanceEntity> Attendances { get; set; } = Enumerable.Empty<AttendanceEntity>();
    }
}
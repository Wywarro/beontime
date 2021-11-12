using Beontime.Domain.Enums;
using Beontime.Domain.Events;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Beontime.Domain.Aggregates
{
    public class TimeCard
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [IgnoreDataMember]
        public DateTime Day { get; set; }

        [IgnoreDataMember]
        public TimeCardStatus Status { get; set; } = TimeCardStatus.InvalidStatus;
        [IgnoreDataMember]
        public TimeSpan WorkDuration { get; set; }
        [IgnoreDataMember]
        public TimeSpan BreakDuration { get; set; }
        [IgnoreDataMember]
        public WorkAttendanceCollection WorkAttendances { get; set; } = new();
        [IgnoreDataMember]
        public BreakAttendanceCollection BreakAttendances { get; set; } = new();

        public void Apply(GettingWorkStarted start)
        {
            Day = start.Timestamp.Date;
            WorkAttendances.Add(new Attendance
            {
                Id = start.Id,
                Timestamp = start.Timestamp,
                Status = EntryStatus.In,
            });
        }

        public void Apply(EndOfWork end)
        {
            WorkAttendances.Add(new Attendance
            {
                Id = end.Id,
                Timestamp = end.Timestamp,
                Status = EntryStatus.Out,
            });
        }

        public void Apply(GettingBreakStarted breakStart)
        {
            BreakAttendances.Add(new Attendance
            {
                Id = breakStart.Id,
                Timestamp = breakStart.Timestamp,
                Status = EntryStatus.BreakStart,
            });
        }

        public void Apply(EndOfBreak breakEnd)
        {
            BreakAttendances.Add(new Attendance
            {
                Id = breakEnd.Id,
                Timestamp = breakEnd.Timestamp,
                Status = EntryStatus.BreakEnd,
            });
        }

        public void Apply(AttendanceEntryDeleted attDeleted)
        {
            WorkAttendances = new WorkAttendanceCollection(WorkAttendances
                .Where(x => x.Id != attDeleted.Id));
            BreakAttendances = new BreakAttendanceCollection(BreakAttendances
                .Where(x => x.Id != attDeleted.Id));
        }
    }
}
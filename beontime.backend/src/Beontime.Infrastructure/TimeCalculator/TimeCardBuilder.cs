namespace Beontime.Infrastructure.TimeCalculator
{
    using System;
    using System.Linq;
    using Application.Common.Interfaces;
    using Application.Extensions;
    using Domain.Aggregates;
    using Domain.Entities;
    using Domain.Enums;

    public class TimeCardBuilder
    {
        private readonly DateTime now;
        private readonly TimeCard timeCard;
        
        private readonly int NearestMinutes = 5;
        
        public TimeCardBuilder(WorkdayEntity workday, IDateTimeService dateTimeService)
        {
            now = dateTimeService.Now;
            timeCard = new TimeCard
            {
                Day = workday.Day,
                Attendances = workday.Attendances
                    .OrderBy(x => x.Timestamp),
            };
        }

        public void CalculateWorkingDuration()
        {
            timeCard.WorkDuration = CalculateDuration(EntryStatus.In, EntryStatus.Out);
        }
        
        public void CalculateBreakDuration()
        {
            timeCard.BreakDuration = CalculateDuration(EntryStatus.BreakStart, EntryStatus.BreakEnd);
        }

        private TimeSpan CalculateDuration(EntryStatus entryStart, EntryStatus entryEnd)
        {
            var attendanceNow = new Lazy<AttendanceEntity>(() => new AttendanceEntity
            {
                Timestamp = now,
                Status = entryEnd,
            });

            var duration = timeCard.Attendances
                .Where(att => att.Status == entryStart)
                .Select(att => new
                {
                    Start = att,
                    End = timeCard.Attendances
                        .SkipWhile(x => x != att)
                        .FirstOrDefault(endAtt => endAtt.Status == entryEnd) ?? attendanceNow.Value,
                })
                .Select(startEndAttendance => startEndAttendance.End.Timestamp - startEndAttendance.Start.Timestamp)
                .Where(workDuration => workDuration.Ticks >= 0)
                .Aggregate(TimeSpan.Zero, (current, workDuration) => current + workDuration);

            return duration.RoundToNearestMinutes(NearestMinutes);
        }

        public TimeCard GetTimeCard()
        {
            return timeCard;
        }
    }
}
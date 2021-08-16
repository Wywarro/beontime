namespace Beontime.Infrastructure.TimeCalculator
{
    using System;
    using System.Linq;
    using Application.Common.Extensions;
    using Application.Common.Interfaces;
    using Domain.Aggregates;
    using Domain.Entities;
    using Domain.Enums;
    using TimeCardBuilderInterfaces;

    public class TimeCardBuilder : 
        IInitialBuilder, IBreakWorkCalculator,
        IWorkDurationCalculator, ITimeCardBuilder
    {
        private readonly IDateTimeService dateTimeService;
        private DateTime now;
        private TimeCard timeCard = new();

        private const int NearestMinutes = 5;

        public TimeCardBuilder(IDateTimeService dateTimeService)
        {
            this.dateTimeService = dateTimeService;

            Reset();
        }

        private void Reset()
        {
            now = dateTimeService.Now;
            timeCard = new TimeCard();
        }

        public IBreakWorkCalculator BuildInitialData(WorkdayEntity workday)
        {
            timeCard.Day = workday.Day;
            timeCard.Attendances = workday.Attendances.OrderBy(x => x.Timestamp);

            return this;
        }
        
        public IWorkDurationCalculator CalculateBreakDuration()
        {
            timeCard.BreakDuration = CalculateDuration(EntryStatus.BreakStart, EntryStatus.BreakEnd);

            return this;
        }

        public ITimeCardBuilder CalculateWorkingDuration()
        {
            timeCard.WorkDuration = CalculateDuration(EntryStatus.In, EntryStatus.Out);
            timeCard.WorkDuration -= timeCard.BreakDuration;

            return this;
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
            var result = timeCard;
            
            Reset();
            
            return result;
        }
    }
}
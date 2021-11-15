using System;
using System.Collections.Generic;
using System.Linq;
using Beontime.Application.Common.Extensions;
using Beontime.Application.Common.Interfaces;
using Beontime.Domain.Aggregates;
using Beontime.Domain.Entities;
using Beontime.Domain.Enums;
using Beontime.Infrastructure.TimeCalculator.TimeCardBuilderInterfaces;

namespace Beontime.Infrastructure.TimeCalculator
{
    public class TimeCardBuilder :
        ITimeCardBuilder,
        IInitialBuilder, IBreakWorkCalculator,
        IWorkDurationCalculator, IStatusResolver
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
            now = dateTimeService.GetDateTimeNow();
            timeCard = new TimeCard();
        }

        public IBreakWorkCalculator BuildInitialData(TimeCard timeCard)
        {
            this.timeCard = timeCard;

            return this;
        }
        
        public IWorkDurationCalculator CalculateBreakDuration()
        {
            timeCard.BreakDuration = CalculateDuration(
                timeCard.BreakAttendances,
                EntryStatus.BreakStart,
                EntryStatus.BreakEnd);

            return this;
        }

        public IStatusResolver CalculateWorkingDuration()
        {
            timeCard.WorkDuration = CalculateDuration(
                timeCard.WorkAttendances,
                EntryStatus.In,
                EntryStatus.Out);
            timeCard.WorkDuration -= timeCard.BreakDuration;

            return this;
        }

        private TimeSpan CalculateDuration(
            IEnumerable<Attendance> attendances,
            EntryStatus entryStart,
            EntryStatus entryEnd)
        {
            var attendanceNow = new Lazy<Attendance>(() => new Attendance
            {
                Timestamp = now,
                Status = entryEnd,
            });

            var duration = attendances
                .Where(att => att.Status == entryStart)
                .Select(att => new
                {
                    Start = att,
                    End = attendances
                        .SkipWhile(x => x != att)
                        .FirstOrDefault(endAtt => endAtt.Status == entryEnd) ?? attendanceNow.Value,
                })
                .Select(startEndAttendance =>
                    startEndAttendance.End.Timestamp - startEndAttendance.Start.Timestamp)
                .Where(workDuration => workDuration.Ticks >= 0)
                .Aggregate(TimeSpan.Zero, (current, workDuration) => current + workDuration);

            return duration.RoundToNearestMinutes(NearestMinutes);
        }

        public ITimeCardBuilder SetTimeCardStatus()
        {
            var handler = WorkdayStatusChain.GenerateStatus(timeCard, now);
            var result = handler.Handle();
            timeCard.Status = (TimeCardStatus) result!;

            return this;
        }

        public TimeCard GetTimeCard()
        {
            var result = timeCard;
            
            Reset();
            
            return result;
        }
    }
}
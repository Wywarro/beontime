using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using System;

namespace Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers
{
    internal sealed class TodayInvalidLogsStatusHandler : GenericStatusChainHandler
    {
        public TodayInvalidLogsStatusHandler(TimeCard timeCard, DateTime now)
            : base(timeCard, now)
        { }

        protected override TimeCardStatus StatusToSet => TimeCardStatus.InvalidLogs;

        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    IsWorkdayToday &&
                    WorkAttendances.Count > 0 &&
                    WorkAttendances.AreAttendancesInvalid() &&
                    BreakAttendances.AreAttendancesInvalid()
                };
            }
        }
    }
}

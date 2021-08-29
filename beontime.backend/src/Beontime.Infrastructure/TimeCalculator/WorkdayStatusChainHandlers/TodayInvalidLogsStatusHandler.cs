using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using Beontime.Infrastructure.TimeCalculator.AttendanceValidators;
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
                    IsWorkdayToday ||
                    Attendances.Count > 0 ||
                    Attendances.AreAttendancesInvalid()
                };
            }
        }
    }
}

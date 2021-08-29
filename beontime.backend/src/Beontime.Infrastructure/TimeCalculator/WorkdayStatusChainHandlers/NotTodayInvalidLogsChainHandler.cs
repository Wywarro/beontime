using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using Beontime.Infrastructure.TimeCalculator.AttendanceValidators;
using System;

namespace Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers
{
    internal sealed class NotTodayInvalidLogsChainHandler : GenericStatusChainHandler
    {
        public NotTodayInvalidLogsChainHandler(TimeCard timeCard, DateTime now)
            : base(timeCard, now)
        { }

        protected override TimeCardStatus StatusToSet => TimeCardStatus.InvalidLogs;

        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    (!IsWorkdayToday &&
                    Attendances.Count > 0 &&
                    Attendances.Count % 2 != 0) ||
                    Attendances.AreAttendancesInvalid() ||
                    Ins - Outs != 0 ||
                    BreakStarts - BreakEnds != 0
                };
            }
        }
    }
}

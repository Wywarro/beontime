using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
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
                    WorkAttendances.Count > 0 &&
                    WorkAttendances.Count % 2 != 0 &&
                    BreakAttendances.Count % 2 != 0) ||
                    WorkAttendances.AreAttendancesInvalid() ||
                    BreakAttendances.AreAttendancesInvalid() ||
                    WorkAttendances.InCount - WorkAttendances.OutCount != 0 ||
                    BreakAttendances.BreakStartCount - BreakAttendances.BreakEndCount != 0
                };
            }
        }
    }
}

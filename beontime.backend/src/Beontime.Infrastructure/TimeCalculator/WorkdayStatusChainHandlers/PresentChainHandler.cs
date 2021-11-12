using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using System;

namespace Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers
{
    internal sealed class PresentChainHandler : GenericStatusChainHandler
    {
        public PresentChainHandler(TimeCard timeCard, DateTime now)
            : base(timeCard, now)
        { }

        protected override TimeCardStatus StatusToSet => TimeCardStatus.Present;

        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    WorkAttendances.Count > 0,
                    WorkAttendances.Count % 2 == 1,
                    IsWorkdayToday,
                    WorkAttendances.InCount - WorkAttendances.OutCount == 1,
                    BreakAttendances.BreakStartCount - BreakAttendances.BreakEndCount == 0,
                };
            }
        }
    }
}

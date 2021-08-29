using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using System;

namespace Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers
{
    internal sealed class BreakChainHandler : GenericStatusChainHandler
    {
        public BreakChainHandler(TimeCard timeCard, DateTime now)
            : base(timeCard, now)
        { }

        protected override TimeCardStatus StatusToSet => TimeCardStatus.Break;

        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    Attendances.Count > 0,
                    IsWorkdayToday,
                    Ins - Outs == 1,
                    BreakStarts - BreakEnds == 1
                };
            }
        }
    }
}

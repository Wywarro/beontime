using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using System;

namespace Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers
{

    internal sealed class OvertimeDayChainHandler : GenericStatusChainHandler
    {
        public TimeSpan WorkDuration { get; set; }
        public TimeSpan BreakDuration { get; set; }

        public OvertimeDayChainHandler(TimeCard timeCard, DateTime now)
            : base(timeCard, now)
        {
            WorkDuration = timeCard.WorkDuration;
            BreakDuration = timeCard.BreakDuration;
        }

        protected override TimeCardStatus StatusToSet => TimeCardStatus.Overtime;

        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    WorkDuration.TotalHours > desiredHoursPerDay,
                };
            }
        }
    }
}

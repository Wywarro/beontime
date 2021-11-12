using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using System;

namespace Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers
{

    internal sealed class UndertimeDayChainHandler : GenericStatusChainHandler
    {
        public TimeSpan WorkDuration { get; set; }
        public TimeSpan BreakDuration { get; set; }

        public UndertimeDayChainHandler(TimeCard timeCard, DateTime now)
            : base(timeCard, now)
        {
            WorkDuration = timeCard.WorkDuration;
            BreakDuration = timeCard.BreakDuration;
        }

        protected override TimeCardStatus StatusToSet => TimeCardStatus.Undertime;

        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    WorkDuration.TotalHours < desiredHoursPerDay,
                };
            }
        }
    }
}

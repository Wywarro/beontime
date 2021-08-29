using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using System;

namespace Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers
{
    internal sealed class FallbackStatusChainHandler : GenericStatusChainHandler
    {
        public FallbackStatusChainHandler(TimeCard timeCard, DateTime now)
            : base(timeCard, now)
        { }

        protected override TimeCardStatus StatusToSet => TimeCardStatus.InvalidStatus;

        protected override bool[] Conditions { get => new bool[] { true }; }
    }
}

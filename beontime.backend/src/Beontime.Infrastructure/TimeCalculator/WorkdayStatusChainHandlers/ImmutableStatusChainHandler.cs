using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using System;

namespace Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers
{
    internal sealed class ImmutableStatusChainHandler : GenericStatusChainHandler
    {
        public WorkdayStatus? Status { get; set; }
        public TimeCardStatus TimeCardStatus { get; set; }

        public ImmutableStatusChainHandler(TimeCard timeCard, DateTime now)
            : base(timeCard, now)
        {
            TimeCardStatus = timeCard.Status;
            Status = timeCard.Status as WorkdayStatus;
        }

        protected override TimeCardStatus StatusToSet => TimeCardStatus;

        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    Status is not null,
                };
            }
        }
    }
}

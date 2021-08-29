using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using System;

namespace Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers
{
    internal sealed class NotYetAtWorkChainHandler : GenericStatusChainHandler
    {
        public NotYetAtWorkChainHandler(TimeCard timeCard, DateTime now)
            : base(timeCard, now)
        { }

        protected override TimeCardStatus StatusToSet => TimeCardStatus.NotAtWorkYet;

        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    (Now - WorkdayStamp).TotalHours < hoursInDay + countAsAbsenceAt,
                    Attendances.Count == 0
                };
            }
        }
    }
}

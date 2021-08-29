using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using System;

namespace Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers
{
    internal sealed class UnexcusedAbsenceChainHandler : GenericStatusChainHandler
    {
        public UnexcusedAbsenceChainHandler(TimeCard timeCard, DateTime now)
            : base(timeCard, now)
        { }

        protected override TimeCardStatus StatusToSet => TimeCardStatus.UnexcusedAbsence;

        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    !IsWorkdayToday,
                    (Now - WorkdayStamp).TotalHours > hoursInDay + countAsAbsenceAt,
                    Attendances.Count == 0
                };
            }
        }
    }
}

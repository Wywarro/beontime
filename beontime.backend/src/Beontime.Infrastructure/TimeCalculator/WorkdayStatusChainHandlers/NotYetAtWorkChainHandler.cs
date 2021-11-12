using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using System;
using System.Linq;

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
                    IsNotAtWorkYet(),
                    WorkAttendances.Count > 0
                };
            }
        }

        private bool IsNotAtWorkYet()
        {
            var firstWorkStamp = WorkAttendances.FirstOrDefault();
            if (firstWorkStamp is null)
            {
                return false;
            }

            return (Now - firstWorkStamp.Timestamp).TotalHours < 0;
        }
    }
}

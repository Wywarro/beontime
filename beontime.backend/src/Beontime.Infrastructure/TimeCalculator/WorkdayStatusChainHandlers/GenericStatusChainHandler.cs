using Beontime.Application.Common.Abstractions;
using Beontime.Domain.Aggregates;
using Beontime.Domain.Entities;
using Beontime.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Beontime.Infrastructure.TimeCalculator.WorkdayStatusChainHandlers
{
    internal abstract class GenericStatusChainHandler : ChainHandler
    {
        protected DateTime Now { get; set; }
        protected DateTime TimeCardDay { get; }
        public WorkAttendanceCollection WorkAttendances { get; set; } = new();
        public BreakAttendanceCollection BreakAttendances { get; set; } = new();

        protected bool IsWorkdayToday => TimeCardDay.Date == Now.Date;

        protected abstract TimeCardStatus StatusToSet { get; }
        protected abstract bool[] Conditions { get; }

        protected int hoursInDay = 24;
        protected int countAsAbsenceAt = 9;
        protected int desiredHoursPerDay = 8;

        public GenericStatusChainHandler(TimeCard timeCard, DateTime now)
        {
            Now = now;
            WorkAttendances = timeCard.WorkAttendances;
            BreakAttendances = timeCard.BreakAttendances;
            TimeCardDay = timeCard.Day;
        }

        public override object? Handle()
        {
            if (Conditions.All(cond => cond))
                return StatusToSet;

            return base.Handle();
        }
    }
}

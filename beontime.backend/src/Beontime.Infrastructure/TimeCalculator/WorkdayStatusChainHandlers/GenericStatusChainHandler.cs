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
        protected DateTime WorkdayStamp { get; }
        protected List<AttendanceEntity> Attendances { get; set; }

        protected bool IsWorkdayToday => WorkdayStamp.Date == Now.Date;

        protected int Ins => 
            Attendances.Where(att => att.Status == EntryStatus.In).Count();
        protected int Outs => 
            Attendances.Where(att => att.Status == EntryStatus.Out).Count();
        protected int BreakStarts => 
            Attendances.Where(att => att.Status == EntryStatus.BreakStart).Count();
        protected int BreakEnds => 
            Attendances.Where(att => att.Status == EntryStatus.BreakEnd).Count();

        protected abstract TimeCardStatus StatusToSet { get; }
        protected abstract bool[] Conditions { get; }

        protected int hoursInDay = 24;
        protected int countAsAbsenceAt = 9;

        public GenericStatusChainHandler(TimeCard timeCard, DateTime now)
        {
            Now = now;
            Attendances = timeCard.Attendances.ToList();
            WorkdayStamp = timeCard.Day;
        }

        public override object? Handle()
        {
            if (Conditions.All(cond => cond))
                return StatusToSet;

            return base.Handle();
        }
    }
}

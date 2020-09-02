using BEonTime.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using static BEonTime.Services.TimeCalculator.AttendanceValidatorFactory;

namespace BEonTime.Services.TimeCalculator
{
    public static class WorkdayStatusValidator
    {
        public static ChainHandler GenerateStatus(Workday workday, DateTime now)
        {
            var presentHandler = new PresentHandler(workday, now);
            var breakHandler = new BreakStatusHandler(workday, now);
            var rdyToCalcHandler = new ReadyToCalcStatusHandler(workday, now);
            var unexcusedAbsenceHandler = new UnexcusedAbsenceStatusHandler(workday, now);
            var invalidLogsHandler = new InvalidLogsStatusHandler(workday, now);
            var invalidStatusHandler = new InvalidStatusHandler(workday, now);

            presentHandler.SetNext(breakHandler).SetNext(rdyToCalcHandler).SetNext(unexcusedAbsenceHandler)
                .SetNext(invalidLogsHandler).SetNext(invalidStatusHandler);

            return presentHandler;
        }
    }

    public interface IHandler
    {
        IHandler SetNext(IHandler handler);
        object Handle();
    }

    public abstract class ChainHandler : IHandler
    {
        public ChainHandler(Workday workday, DateTime now)
        {
            List<Attendance> attendances = workday.Attendances;

            IsWorkdayToday = workday.Datestamp.Date == now.Date;
            Ins = attendances.FindAll(att => att.Status == EntryMode.In).Count;
            Outs = attendances.FindAll(att => att.Status == EntryMode.Out).Count;
            BreakStarts = attendances.FindAll(att => att.Status == EntryMode.BreakStart).Count;
            BreakEnds = attendances.FindAll(att => att.Status == EntryMode.BreakEnd).Count;
        }

        protected bool IsWorkdayToday { get; }
        protected int Ins { get; set; }
        protected int Outs { get; set; }
        protected int BreakStarts { get; set; }
        protected int BreakEnds { get; set; }
        protected abstract bool[] Conditions { get; }
        protected abstract WorkdayStatus StatusToSet { get; }

        private IHandler nextHandler;

        public IHandler SetNext(IHandler handler)
        {
            nextHandler = handler;
            return handler;
        }
        
        public virtual object Handle()
        {
            if (nextHandler != null)
            {
                return nextHandler.Handle();
            }
            else
            {
                return null;
            }
        }
    }

    public class PresentHandler : ChainHandler
    {
        public PresentHandler(Workday workday, DateTime now)
            : base(workday, now)
        { }
        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    IsWorkdayToday,
                    Ins - Outs == 1,
                    BreakStarts - BreakEnds == 0
                };
            }
        }
        protected override WorkdayStatus StatusToSet => WorkdayStatus.Present;

        public override object Handle()
        {
            if (Conditions.All(cond => cond))
                return StatusToSet;
            else
                return base.Handle();
        }
    }

    public class BreakStatusHandler : ChainHandler
    {
        public BreakStatusHandler(Workday workday, DateTime now)
            : base(workday, now)
        { }
        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    IsWorkdayToday,
                    Ins - Outs == 1,
                    BreakStarts - BreakEnds == 1
                };
            }
        }
        protected override WorkdayStatus StatusToSet => WorkdayStatus.Break;

        public override object Handle()
        {
            if (Conditions.All(cond => cond))
                return StatusToSet;
            else
                return base.Handle();
        }
    }

    public class ReadyToCalcStatusHandler : ChainHandler
    {
        public ReadyToCalcStatusHandler(Workday workday, DateTime now)
            : base(workday, now)
        { }
        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    Ins > 0,
                    Ins - Outs == 0,
                    BreakStarts - BreakEnds == 0
                };
            }
        }
        protected override WorkdayStatus StatusToSet => WorkdayStatus.ReadyToCalc;

        public override object Handle()
        {
            if (Conditions.All(cond => cond))
                return StatusToSet;
            else
                return base.Handle();
        }
    }

    public class UnexcusedAbsenceStatusHandler : ChainHandler
    {
        private DateTime Now { get; set; }
        private int AttsCount { get; set; }
        public UnexcusedAbsenceStatusHandler(Workday workday, DateTime now)
            : base(workday, now)
        {
            Now = now;
            AttsCount = workday.Attendances.Count;
        }
        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    !IsWorkdayToday,
                    Now.TimeOfDay.Hours > 9,
                    AttsCount== 0
                };
            }
        }
        protected override WorkdayStatus StatusToSet => WorkdayStatus.UnexcusedAbsence;

        public override object Handle()
        {
            if (Conditions.All(cond => cond))
                return StatusToSet;
            else
                return base.Handle();
        }
    }

    public class InvalidLogsStatusHandler : ChainHandler
    {
        private List<Attendance> Attendances { get; set; }
        public InvalidLogsStatusHandler(Workday workday, DateTime now)
            : base(workday, now)
        {
            Attendances = workday.Attendances;
        }
        protected override bool[] Conditions
        {
            get
            {
                return new bool[]
                {
                    Attendances.Count > 0,
                    !IsWorkdayToday && Attendances.Count % 2 != 0,
                    (Attendances?.First()?.Status ?? EntryMode.In) != EntryMode.In
                }.Concat(CheckAtts()).ToArray();
            }
        }
        protected override WorkdayStatus StatusToSet => WorkdayStatus.InvalidLogs;

        private bool[] CheckAtts()
        {
            List<bool> attConds = new List<bool>();
            for (int i = 0; i < Attendances.Count - 1; i++)
            {
                var attendance = Attendances[i];

                var nextAttendance = Attendances.SkipWhile(x => x != attendance)
                    .Skip(1).DefaultIfEmpty(Attendances[0]).FirstOrDefault();

                AttValidator validator = GenerateAttValidator(attendance);
                bool nextAttValid = validator.AllowedNextModes
                    ?.Any(mod => mod == nextAttendance.Status) ?? true;
                attConds.Add(nextAttValid);
            }
            return attConds.ToArray();
        }

        public override object Handle()
        {
            if (Conditions.All(cond => cond))
                return StatusToSet;
            else
                return base.Handle();
        }
    }

    public class InvalidStatusHandler : ChainHandler
    {
        public InvalidStatusHandler(Workday workday, DateTime now)
            : base(workday, now)
        { }
        protected override bool[] Conditions { get => new bool[] { true }; }
        protected override WorkdayStatus StatusToSet => WorkdayStatus.InvalidStatus;

        public override object Handle()
        {
            if (Conditions.All(cond => cond))
                return StatusToSet;
            else
                return base.Handle();
        }
    }
}

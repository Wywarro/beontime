using BEonTime.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BEonTime.Services.TimeCalculator
{
    public static class WorkdayStatusValidatorFactory
    {
        public static Queue<WorkdayStatusValidator> GenerateWorkdayStatus(Workday workday, DateTime now)
        {
            return new Queue<WorkdayStatusValidator>
            (
                new WorkdayStatusValidator[]
                {
                    new PresentStatus(workday, now),
                    new BreakStatus(workday, now),
                    new ReadyToCalcStatus(workday, now),
                    new UnexcusedAbsenceStatus(workday, now),
                    new InvalidLogsStatus(workday, now),
                    new InvalidStatus(workday, now)
                }
            );

        }

        public abstract class WorkdayStatusValidator
        {
            public WorkdayStatusValidator(Workday workday, DateTime now)
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
            public abstract bool[] Conditions { get; }
            public abstract WorkdayStatus StatusToSet { get; }
        }

        private class PresentStatus : WorkdayStatusValidator
        {
            public PresentStatus(Workday workday, DateTime now)
                : base(workday, now)
            { }
            public override bool[] Conditions
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
            public override WorkdayStatus StatusToSet => WorkdayStatus.Present;
        }

        private class BreakStatus : WorkdayStatusValidator
        {
            public BreakStatus(Workday workday, DateTime now)
                : base(workday, now)
            { }
            public override bool[] Conditions
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
            public override WorkdayStatus StatusToSet => WorkdayStatus.Break;
        }

        private class ReadyToCalcStatus : WorkdayStatusValidator
        {
            public ReadyToCalcStatus(Workday workday, DateTime now)
                : base(workday, now)
            { }
            public override bool[] Conditions
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
            public override WorkdayStatus StatusToSet => WorkdayStatus.ReadyToCalc;
        }

        private class UnexcusedAbsenceStatus : WorkdayStatusValidator
        {
            private DateTime Now { get; set; }
            private int AttsCount { get; set; }
            public UnexcusedAbsenceStatus(Workday workday, DateTime now)
                : base(workday, now)
            {
                Now = now;
                AttsCount = workday.Attendances.Count;
            }
            public override bool[] Conditions
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
            public override WorkdayStatus StatusToSet => WorkdayStatus.UnexcusedAbsence;
        }

        private class InvalidLogsStatus : WorkdayStatusValidator
        {
            private int AttsCount { get; set; }
            public InvalidLogsStatus(Workday workday, DateTime now)
                : base(workday, now)
            {
                AttsCount = workday.Attendances.Count;
            }
            public override bool[] Conditions
            {
                get
                {
                    return new bool[]
                    {
                        !IsWorkdayToday,
                        AttsCount % 2 != 0
                    };
                }
            }
            public override WorkdayStatus StatusToSet => WorkdayStatus.InvalidLogs;
        }

        private class InvalidStatus : WorkdayStatusValidator
        {
            public InvalidStatus(Workday workday, DateTime now)
                : base(workday, now)
            { }
            public override bool[] Conditions { get => null; }
            public override WorkdayStatus StatusToSet => WorkdayStatus.InvalidStatus;
        }
    }
}

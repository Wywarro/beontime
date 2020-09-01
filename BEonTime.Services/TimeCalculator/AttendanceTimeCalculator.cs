using BIMonTime.Data.Entities;
using BIMonTime.Services.DateTimeProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BIMonTime.Services.TimeCalculator
{
    public interface IAttendanceTimeCalculator
    {
        void GetWorkingTime(Workday workday);
    }

    public class AttendanceTimeCalculator : IAttendanceTimeCalculator
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly TimeSpan actualTime;
        private readonly DateTime today;

        public AttendanceTimeCalculator(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
            actualTime = dateTimeProvider.GetDateTimeNow().TimeOfDay;
            today = dateTimeProvider.GetDateTimeNow().Date;
        }

        public void GetWorkingTime(Workday workday)
        {
            try
            {
                Validate(workday);
                SetStatus(workday);
            }
            catch (Exception)
            {
                workday.Status = WorkdayStatus.InvalidLogs;
            }

            workday.BreakDuration = TimeSpan.FromSeconds(60);
            workday.WorkDuration = TimeSpan.FromSeconds(50);
        }

        private void Validate(Workday workday)
        {
            List<Attendance> attendances = workday.Attendances;

            for (int i = 0; i < attendances.Count - 1; i++)
            {
                var attendance = attendances[i];
                if (i == 0 && attendance.Status != EntryMode.In)
                {
                    throw new Exception("First Entry's mode needs to be IN");
                }
                var nextAttendance = attendances.SkipWhile(x => x != attendance)
                    .Skip(1).DefaultIfEmpty(attendances[0]).FirstOrDefault();

                AttValidator validator = AttValidatorFactory.GenerateAttValidator(attendance);
                if (validator.AllowedNextModes.All(mod => mod != nextAttendance.Status))
                {
                    throw new Exception("First Entry's mode needs to be IN");
                }
            }
        }

        private void SetStatus(Workday workday)
        {
            bool isWorkdayToday = workday.Datestamp.Date == today;
            List<Attendance> attendances = workday.Attendances;

            var attIns = attendances.FindAll(att => att.Status == EntryMode.In);
            var attInsCount = attIns.Count;
            var attOuts = attendances.FindAll(att => att.Status == EntryMode.Out);
            var attOutsCount = attOuts.Count;
            var attBreakStarts = attendances.FindAll(att => att.Status == EntryMode.BreakStart);
            var attBreakStartsCount = attBreakStarts.Count;
            var attBreakEnds = attendances.FindAll(att => att.Status == EntryMode.BreakEnd);
            var attBreakEndsCount = attBreakEnds.Count;

            if (isWorkdayToday)
            {
                if (attInsCount - attOutsCount > 1 && ((attBreakStartsCount - attBreakEndsCount) == 0))
                {
                    workday.Status = WorkdayStatus.Present;
                }
                else if (attInsCount - attOutsCount > 1 && ((attBreakStartsCount - attBreakEndsCount) == 1))
                {
                    workday.Status = WorkdayStatus.Break;
                }
                else if (attInsCount > 0 && attInsCount == attOutsCount && attBreakStartsCount == attBreakEndsCount)
                {
                    workday.Status = WorkdayStatus.NormalDay;
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                if (attendances.Count == 0)
                {
                    if (actualTime.Hours > 9)
                    {
                        workday.Status = WorkdayStatus.UnexcusedAbsence;
                    }
                    else
                    {
                        workday.Status = WorkdayStatus.InvalidStatus;
                    }
                }
                else if (attInsCount != attOutsCount)
                {
                    workday.Status = WorkdayStatus.InvalidLogs;
                }
            }
        }

        private static class AttValidatorFactory
        {
            public static AttValidator GenerateAttValidator(Attendance att)
            {
                return att.Status switch
                {
                    EntryMode.In => new InAtt(),
                    EntryMode.BreakStart => new BreakStartAtt(),
                    EntryMode.BreakEnd => new BreakEndAtt(),
                    EntryMode.Out => new OutAtt(),

                    _ => throw new InvalidEnumArgumentException(),
                };
            }
        }

        private abstract class AttValidator
        {
            public abstract EntryMode[] AllowedNextModes { get; }
        }

        private class InAtt : AttValidator
        {
            public override EntryMode[] AllowedNextModes => new EntryMode[] { EntryMode.BreakStart, EntryMode.Out };
        }

        private class BreakStartAtt : AttValidator
        {
            public override EntryMode[] AllowedNextModes => new EntryMode[] { EntryMode.BreakEnd };
        }

        private class BreakEndAtt : AttValidator
        {
            public override EntryMode[] AllowedNextModes => new EntryMode[] { EntryMode.BreakStart, EntryMode.Out };
        }

        private class OutAtt : AttValidator
        {
            public override EntryMode[] AllowedNextModes => new EntryMode[] { EntryMode.In };
        }

        private abstract class WorkdayStatusValidator
        {
            public abstract bool[] Contitions { get; set; }
        }
    }
}

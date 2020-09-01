using BEonTime.Data.Entities;
using BEonTime.Services.DateTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using static BEonTime.Services.TimeCalculator.AttendanceValidatorFactory;
using static BEonTime.Services.TimeCalculator.WorkdayStatusValidatorFactory;

namespace BEonTime.Services.TimeCalculator
{
    public interface IAttendanceTimeCalculator
    {
        void GetWorkingTime(Workday workday);
    }

    public class AttendanceTimeCalculator : IAttendanceTimeCalculator
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly DateTime now;

        public AttendanceTimeCalculator(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
            now = dateTimeProvider.GetDateTimeNow();
        }

        public void GetWorkingTime(Workday workday)
        {
            try
            {
                ValidateAttendancesEntryModes(workday);
                SetStatus(workday);
            }
            catch (Exception)
            {
                workday.Status = WorkdayStatus.InvalidLogs;
            }

            workday.BreakDuration = TimeSpan.FromSeconds(60);
            workday.WorkDuration = TimeSpan.FromSeconds(50);
        }

        private void ValidateAttendancesEntryModes(Workday workday)
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

                AttValidator validator = GenerateAttValidator(attendance);
                bool nextAttNotValid = validator.AllowedNextModes?.All(mod => mod != nextAttendance.Status) ?? true;
                if (nextAttNotValid)
                {
                    throw new Exception("After: In => BreakStart or Our; " +
                        "BreakStart => BreakEnd; " +
                        "BreakEnd => BreakStart or Out;" +
                        "Out => In");
                }
            }
        }

        private void SetStatus(Workday workday)
        {
            var validatorQueue = GenerateWorkdayStatus(workday, now);
            for (int i = 0; i < validatorQueue.Count; i++)
            {
                WorkdayStatusValidator validator = validatorQueue.Dequeue();
                if (validator.Conditions.All(cond => cond == true))
                {
                    workday.Status = validator.StatusToSet;
                    return;
                }
            }
        }
    }
}

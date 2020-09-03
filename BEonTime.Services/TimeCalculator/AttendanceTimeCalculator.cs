using BEonTime.Data.Entities;
using BEonTime.Services.DateTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using static BEonTime.Services.TimeCalculator.AttendanceValidatorFactory;
using static BEonTime.Services.TimeCalculator.WorkdayStatusValidator;

namespace BEonTime.Services.TimeCalculator
{
    public interface IAttendanceTimeCalculator
    {
        void GetWorkingTime(Workday workday);
    }

    public class AttendanceTimeCalculator : IAttendanceTimeCalculator
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public AttendanceTimeCalculator(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
        }

        public void GetWorkingTime(Workday workday)
        {
            if (workday.IsStatusMutable())
            {
                SetStatus(workday);
            }

            workday.BreakDuration = TimeSpan.FromSeconds(60);
            workday.WorkDuration = TimeSpan.FromSeconds(50);
        }

        private void SetStatus(Workday workday)
        {
            DateTime now = dateTimeProvider.GetDateTimeNow();

            ChainHandler handler = GenerateStatus(workday, now);
            var result = handler.Handle();
            workday.Status = (WorkdayStatus) result;
        }
    }
}

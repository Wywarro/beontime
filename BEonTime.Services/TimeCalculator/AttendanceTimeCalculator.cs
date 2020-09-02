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
        private readonly DateTime now;

        public AttendanceTimeCalculator(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
            now = dateTimeProvider.GetDateTimeNow();
        }

        public void GetWorkingTime(Workday workday)
        {
            SetStatus(workday);

            workday.BreakDuration = TimeSpan.FromSeconds(60);
            workday.WorkDuration = TimeSpan.FromSeconds(50);
        }

        private void SetStatus(Workday workday)
        {
            ChainHandler handler = GenerateStatus(workday, now);
            var result = handler.Handle();
            workday.Status = (WorkdayStatus) result;
        }
    }
}

using BIMonTime.Data.Entities;
using BIMonTime.Services.DateTimeProvider;
using System;

namespace BIMonTime.Services.TimeCalculator
{
    public interface IAttendanceTimeCalculator
    {
        TimeSpan GetWorkingTime(Workday workday, out TimeSpan breaktime, out WorkdayStatus status);
    }

    public class AttendanceTimeCalculator : IAttendanceTimeCalculator
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public AttendanceTimeCalculator(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
        }

        public TimeSpan GetWorkingTime(Workday workday, out TimeSpan breaktime, out WorkdayStatus status)
        {
            status = WorkdayStatus.Present;

            breaktime = TimeSpan.FromSeconds(60);

            return TimeSpan.FromSeconds(50);
        }
    }
}

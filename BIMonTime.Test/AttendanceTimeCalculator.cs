using BIMonTime.Data.Entities;
using BIMonTime.Services.DateTimeProvider;
using BIMonTime.Services.TimeCalculator;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace BIMonTime.Test
{
    public class AttendanceTimeCalculatorTest
    {
        private readonly IDateTimeProvider mockDateTimeProv;
        private readonly DateTime now = new DateTime(2020, 9, 29, 12, 50, 10);

        public AttendanceTimeCalculatorTest()
        {
            mockDateTimeProv = Mock.Of<IDateTimeProvider>();
            Mock.Get(mockDateTimeProv)
                .Setup(dat => dat.GetDateTimeNow())
                .Returns(now);
        }

        [Fact]
        public void When_only_one_attendance_then_status_is_present()
        {
            Workday workday = new Workday()
            {
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday, out TimeSpan breaktime, out WorkdayStatus status);

            Assert.Equal(expected: WorkdayStatus.Present, actual: status);
        }

        [Fact]
        public void When_only_one_attendance_and_is_today_then_status_is_present()
        {
            Workday workday = new Workday()
            {
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday, out TimeSpan breaktime, out WorkdayStatus status);

            Assert.Equal(expected: WorkdayStatus.Present, actual: status);
        }

        [Fact]
        public void When_only_one_attendance_and_is_not_today_then_status_is_invalidLogs()
        {
            Workday workday = new Workday()
            {
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 28,  8, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday, out TimeSpan breaktime, out WorkdayStatus status);

            Assert.Equal(expected: WorkdayStatus.InvalidLogs, actual: status);
        }
    }
}

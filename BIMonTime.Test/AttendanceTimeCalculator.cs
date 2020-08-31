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
        public void When_istoday_one_in_then_status_is_Present()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.Present, actual: workday.Status);
        }

        [Theory]
        [InlineData(EntryMode.Out)]
        [InlineData(EntryMode.BreakStart)]
        [InlineData(EntryMode.BreakEnd)]
        public void When_istoday_one_other_than_in_then_status_is_InvalidLogs(EntryMode attStatusPayload)
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = attStatusPayload, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.InvalidLogs, actual: workday.Status);
        }

        [Fact]
        public void When_istoday_one_in_one_breakstart_then_status_is_Break()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  10, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.Break, actual: workday.Status);
        }

        [Fact]
        public void When_istoday_one_in_one_out_then_status_is_NormalDay()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.Out, Timestamp = new DateTime(2020, 9, 29,  10, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.NormalDay, actual: workday.Status);
        }

        [Theory]
        [InlineData(EntryMode.In)]
        [InlineData(EntryMode.BreakEnd)]
        public void When_istoday_one_in_one_breakend_or_one_in_then_status_is_InvalidLogs(EntryMode attStatusPayload)
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                    new Attendance()
                    { Status = attStatusPayload, Timestamp = new DateTime(2020, 9, 29,  10, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.InvalidLogs, actual: workday.Status);
        }

        [Fact]
        public void When_istoday_one_in_one_breakstart_one_breakend_then_status_is_Present()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  10, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakEnd, Timestamp = new DateTime(2020, 9, 29,  11, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.Present, actual: workday.Status);
        }

        [Fact]
        public void When_istoday_one_in_one_breakstart_one_out_then_status_is_InvalidLogs()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  10, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.Out, Timestamp = new DateTime(2020, 9, 29,  11, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.InvalidLogs, actual: workday.Status);
        }

        [Fact]
        public void When_istoday_one_in_one_out_one_in_then_status_is_Present()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.Out, Timestamp = new DateTime(2020, 9, 29,  11, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  11, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.Present, actual: workday.Status);
        }

        [Fact]
        public void When_istoday_one_in_one_breakstart_one_breakend_one_out_then_status_is_()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  10, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakEnd, Timestamp = new DateTime(2020, 9, 29,  11, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.Out, Timestamp = new DateTime(2020, 9, 29,  12, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.NormalDay, actual: workday.Status);
        }

        [Fact]
        public void When_isnottoday_one_in_then_status_is_InvalidLogs()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 28),
                Attendances = new List<Attendance>
            {
                new Attendance()
                { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 28,  8, 2, 10) }
            }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.InvalidLogs, actual: workday.Status);
        }

        [Fact]
        public void When_isnottoday_no_attendances_its_after_9_then_status_is_UnexcusedAbsence()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 28),
                Attendances = new List<Attendance>()
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.UnexcusedAbsence, actual: workday.Status);
        }

        [Fact]
        public void When_isnottoday_no_attendances_its_before_9_then_status_is_UnexcusedAbsence()
        {
            DateTime now = new DateTime(2020, 9, 29, 8, 50, 10);
            Mock.Get(mockDateTimeProv)
                .Setup(dat => dat.GetDateTimeNow())
                .Returns(now);

            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 28),
                Attendances = new List<Attendance>()
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.InvalidStatus, actual: workday.Status);
        }
    }
}

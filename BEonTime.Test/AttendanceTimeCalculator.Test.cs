using BEonTime.Data.Entities;
using BEonTime.Services.DateTimeProvider;
using BEonTime.Services.TimeCalculator;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace BEonTime.Test
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


        [Fact]
        public void When_istoday_one_in_multiple_breakStarts_followed_by_breakEnds_then_status_is_Present()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  9, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakEnd, Timestamp = new DateTime(2020, 9, 29,  10, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  11, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakEnd, Timestamp = new DateTime(2020, 9, 29,  12, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  13, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakEnd, Timestamp = new DateTime(2020, 9, 29,  14, 2, 10) },
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

        [Theory]
        [InlineData(WorkdayStatus.VacationLeaveRequested)]
        [InlineData(WorkdayStatus.VacationLeaveApproved)]
        [InlineData(WorkdayStatus.PaidLeave)]
        [InlineData(WorkdayStatus.ParentalLeave)]
        [InlineData(WorkdayStatus.BusinessTripLeave)]
        [InlineData(WorkdayStatus.OvertimeLeave)]
        [InlineData(WorkdayStatus.SicknessLeave)]
        public void When_workday_status_is_immutable_then_status_remains(WorkdayStatus immutableStatus)
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Status = immutableStatus,
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakEnd, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: immutableStatus, actual: workday.Status);
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
        public void When_istoday_one_in_multiple_breakStarts_followed_by_breakEnds_last_is_breakStart_then_status_is_Break()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  9, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakEnd, Timestamp = new DateTime(2020, 9, 29,  10, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  11, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakEnd, Timestamp = new DateTime(2020, 9, 29,  12, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  13, 2, 10) },
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.Break, actual: workday.Status);
        }

        [Fact]
        public void When_istoday_one_in_one_out_then_status_is_ReadyToCalc()
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

            Assert.Equal(expected: WorkdayStatus.ReadyToCalc, actual: workday.Status);
        }

        [Fact]
        public void When_istoday_multiple_ins_followed_by_outs_then_status_is_ReadyToCalc()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.Out, Timestamp = new DateTime(2020, 9, 29,  10, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  11, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.Out, Timestamp = new DateTime(2020, 9, 29,  12, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  13, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.Out, Timestamp = new DateTime(2020, 9, 29,  14, 2, 10) },
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.ReadyToCalc, actual: workday.Status);
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

        [Theory]
        [InlineData(EntryMode.Out)]
        [InlineData(EntryMode.In)]
        [InlineData(EntryMode.BreakStart)]
        public void When_istoday_one_in_one_breakstart_one_out_then_status_is_InvalidLogs(EntryMode attStatusPayload)
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
                    { Status = attStatusPayload, Timestamp = new DateTime(2020, 9, 29,  11, 2, 10) }
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
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  12, 2, 10) }
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

            Assert.Equal(expected: WorkdayStatus.ReadyToCalc, actual: workday.Status);
        }

        [Theory]
        [InlineData(EntryMode.In)]
        [InlineData(EntryMode.Out)]
        [InlineData(EntryMode.BreakStart)]
        [InlineData(EntryMode.BreakEnd)]
        public void When_isnottoday_one_in_then_status_is_InvalidLogs(EntryMode attStatusPayload)
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 9),
                Attendances = new List<Attendance>
            {
                new Attendance()
                { Status = attStatusPayload, Timestamp = new DateTime(2020, 9, 9,  8, 2, 10) }
            }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.InvalidLogs, actual: workday.Status);
        }

        [Theory]
        [InlineData(EntryMode.In)]
        [InlineData(EntryMode.BreakStart)]
        [InlineData(EntryMode.BreakEnd)]
        public void When_isnottoday_one_in_one_other_than_out_then_status_is_InvalidLogs(EntryMode attStatusPayload)
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 9),
                Attendances = new List<Attendance>
            {
                new Attendance()
                { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 9,  8, 2, 10) },
                new Attendance()
                { Status = attStatusPayload, Timestamp = new DateTime(2020, 9, 9,  8, 2, 10) }
            }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.InvalidLogs, actual: workday.Status);
        }

        [Theory]
        [InlineData(EntryMode.In, EntryMode.In, EntryMode.Out, EntryMode.Out)]
        [InlineData(EntryMode.In, EntryMode.Out, EntryMode.BreakStart, EntryMode.BreakEnd)]
        [InlineData(EntryMode.In, EntryMode.BreakStart, EntryMode.Out, EntryMode.BreakEnd)]
        [InlineData(EntryMode.BreakStart, EntryMode.BreakStart, EntryMode.In, EntryMode.Out)]
        [InlineData(EntryMode.In, EntryMode.In, EntryMode.In, EntryMode.In)]
        [InlineData(EntryMode.Out, EntryMode.Out, EntryMode.Out, EntryMode.Out)]
        [InlineData(EntryMode.BreakStart, EntryMode.BreakStart, EntryMode.BreakStart, EntryMode.BreakStart)]
        [InlineData(EntryMode.BreakEnd, EntryMode.BreakEnd, EntryMode.BreakEnd, EntryMode.BreakEnd)]
        public void When_isnottoday_and_different_configs_then_status_is_InvalidLogs(
            EntryMode stStatus, EntryMode ndStatus, EntryMode rdStatus, EntryMode fourthstatus)
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 9),
                Attendances = new List<Attendance>
            {
                new Attendance()
                { Status = stStatus, Timestamp = new DateTime(2020, 9, 9,  8, 2, 10) },
                new Attendance()
                { Status = ndStatus, Timestamp = new DateTime(2020, 9, 9,  8, 2, 10) },
                new Attendance()
                { Status = rdStatus, Timestamp = new DateTime(2020, 9, 9,  8, 2, 10) },
                new Attendance()
                { Status = fourthstatus, Timestamp = new DateTime(2020, 9, 9,  8, 2, 10) }
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
                Datestamp = new DateTime(2020, 9, 9),
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
                Datestamp = new DateTime(2020, 9, 9),
                Attendances = new List<Attendance>()
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.InvalidStatus, actual: workday.Status);
        }
    }
}

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
            Assert.Equal(expected: TimeSpan.FromMinutes(4 * 60 + 50), actual: workday.WorkDuration);
            Assert.Equal(expected: TimeSpan.FromMinutes(0 * 60 + 0), actual: workday.BreakDuration);
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
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  7, 33, 10) },
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
            Assert.Equal(expected: TimeSpan.FromMinutes(2 * 60 + 15), actual: workday.WorkDuration);
            Assert.Equal(expected: TimeSpan.FromMinutes(3 * 60 + 0), actual: workday.BreakDuration);
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
        public void When_istoday_one_early_in_one_breakstart_then_status_is_Break()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  6, 21, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  10, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.Break, actual: workday.Status);
            Assert.Equal(expected: TimeSpan.FromMinutes(3 * 60 + 40), actual: workday.WorkDuration);
            Assert.Equal(expected: TimeSpan.FromMinutes(2 * 60 + 50), actual: workday.BreakDuration);
        }

        [Fact]
        public void When_istoday_one_late_in_one_breakstart_then_status_is_Break()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  8, 21, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  10, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.Break, actual: workday.Status);
            Assert.Equal(expected: TimeSpan.FromMinutes(1 * 60 + 40), actual: workday.WorkDuration);
            Assert.Equal(expected: TimeSpan.FromMinutes(2 * 60 + 50), actual: workday.BreakDuration);
        }

        [Fact]
        public void When_istoday_one_in_multiple_breakStarts_followed_by_breakEnds_last_is_breakStart_with_stamp_after_now_then_status_is_Present()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  5, 59, 10) },
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

            Assert.Equal(expected: WorkdayStatus.Present, actual: workday.Status);
            Assert.Equal(expected: TimeSpan.FromMinutes(4 * 60 + 50), actual: workday.WorkDuration);
            Assert.Equal(expected: TimeSpan.FromMinutes(2 * 60 + 00), actual: workday.BreakDuration);
        }

        [Fact]
        public void When_istoday_one_in_with_stamp_after_now_then_status_is_Present()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  13, 59, 10) },
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.NotAtWorkYet, actual: workday.Status);
            Assert.Equal(expected: TimeSpan.FromMinutes(0 * 60 + 0), actual: workday.WorkDuration);
            Assert.Equal(expected: TimeSpan.FromMinutes(0 * 60 + 0), actual: workday.BreakDuration);
        }

        [Fact]
        public void When_istoday_in_at_8_out_at_10_then_status_is_ReadyToCalc_and_workDuration_2h()
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
            Assert.Equal(expected: TimeSpan.FromMinutes(2 * 60 + 0), actual: workday.WorkDuration);
            Assert.Equal(expected: TimeSpan.FromMinutes(0 * 60 + 0), actual: workday.BreakDuration);
        }

        [Fact]
        public void When_istoday_multiple_ins_followed_by_outs_then_status_is_ReadyToCalc_and_workDuration_is_4h()
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
            Assert.Equal(expected: TimeSpan.FromMinutes(4 * 60 + 0), actual: workday.WorkDuration);
            Assert.Equal(expected: TimeSpan.FromMinutes(0 * 60 + 0), actual: workday.BreakDuration);
        }

        [Fact]
        public void When_istoday_one_in_one_out_one_in_after_now_then_status_is_NotAtWorkYet_and_workDuration_is_6h_45min()
        {
            Workday workday = new Workday()
            {
                Datestamp = new DateTime(2020, 9, 29),
                Attendances = new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  5, 2, 10) },
                    new Attendance()
                    { Status = EntryMode.Out, Timestamp = new DateTime(2020, 9, 29,  11, 49, 10) },
                    new Attendance()
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  13, 2, 10) },
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.NotAtWorkYet, actual: workday.Status);
            Assert.Equal(expected: TimeSpan.FromMinutes(6 * 60 + 45), actual: workday.WorkDuration);
            Assert.Equal(expected: TimeSpan.FromMinutes(0 * 60 + 0), actual: workday.BreakDuration);
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
                    { Status = EntryMode.In, Timestamp = new DateTime(2020, 9, 29,  9, 24, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakStart, Timestamp = new DateTime(2020, 9, 29,  10, 12, 10) },
                    new Attendance()
                    { Status = EntryMode.BreakEnd, Timestamp = new DateTime(2020, 9, 29,  11, 2, 10) }
                }
            };

            IAttendanceTimeCalculator timeCalculator = new AttendanceTimeCalculator(mockDateTimeProv);
            timeCalculator.GetWorkingTime(workday);

            Assert.Equal(expected: WorkdayStatus.Present, actual: workday.Status);
            Assert.Equal(expected: TimeSpan.FromMinutes(2 * 60 + 35), actual: workday.WorkDuration);
            Assert.Equal(expected: TimeSpan.FromMinutes(0 * 60 + 50), actual: workday.BreakDuration);
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
                    { Status = attStatusPayload, Timestamp = new DateTime(2020, 9, 29,  15, 2, 10) }
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
            Assert.Equal(expected: TimeSpan.FromMinutes(3 * 60 + 50), actual: workday.WorkDuration);
            Assert.Equal(expected: TimeSpan.FromMinutes(0 * 60 + 0), actual: workday.BreakDuration);
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
            Assert.Equal(expected: TimeSpan.FromMinutes(3 * 60 + 0), actual: workday.WorkDuration);
            Assert.Equal(expected: TimeSpan.FromMinutes(1 * 60 + 0), actual: workday.BreakDuration);
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

            Assert.Equal(expected: WorkdayStatus.UnexcusedAbsence, actual: workday.Status);
        }

        [Fact]
        public void When_day_after_today_no_attendances_its_before_9_then_status_is_NotAtWorkYet()
        {
            DateTime now = new DateTime(2020, 9, 10, 8, 50, 10);
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

            Assert.Equal(expected: WorkdayStatus.NotAtWorkYet, actual: workday.Status);
        }
    }
}

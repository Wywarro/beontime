using Beontime.Application.Common.Interfaces;
using Beontime.Domain.Aggregates;
using Beontime.Domain.Enums;
using Beontime.Infrastructure.TimeCalculator;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Beontime.Tests.TimeCalculator
{
    public class TimeCardBuilderTests
    {
        private readonly IDateTimeService mockDateTimeService;
        private readonly DateTime now = new(2020, 9, 29, 12, 50, 10);

        public TimeCardBuilderTests()
        {
            mockDateTimeService = Mock.Of<IDateTimeService>();
            Mock.Get(mockDateTimeService)
                .Setup(dat => dat.GetDateTimeNow())
                .Returns(now);
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenNoBreaks()
        {
            var timeCardInit = new TimeCard
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29, 8, 2, 10),
                    },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(4.Hours().And(50.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(0.Minutes()));
            timeCard.Status.ToString().Should().Be(TimeCardStatus.Present.ToString());
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenMultipleBreaks()
        {
            var timeCardInit = new TimeCard
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29, 7, 33, 10)
                    },
                }),
                BreakAttendances = new BreakAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.BreakStart, 
                        Timestamp = new DateTime(2020, 9, 29, 9, 2, 10)
                    },
                    new()
                    {
                        Status = EntryStatus.BreakEnd, 
                        Timestamp = new DateTime(2020, 9, 29, 10, 2, 10)
                    },
                    new()
                    {
                        Status = EntryStatus.BreakStart, 
                        Timestamp = new DateTime(2020, 9, 29, 11, 2, 10)
                    },
                    new()
                    {
                        Status = EntryStatus.BreakEnd, 
                        Timestamp = new DateTime(2020, 9, 29, 12, 2, 10)
                    },
                    new()
                    {
                        Status = EntryStatus.BreakStart, 
                        Timestamp = new DateTime(2020, 9, 29, 13, 2, 10)
                    },
                    new()
                    {
                        Status = EntryStatus.BreakEnd, 
                        Timestamp = new DateTime(2020, 9, 29, 14, 2, 10)
                    },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(2.Hours().And(15.Minutes()));
            timeCard.BreakDuration.Should().Be(3.Hours().And(0.Minutes()));
            timeCard.Status.Should().Be(TimeCardStatus.Present);
        }

        [Fact]
        public void SetTimeCardStatus_ShouldSetInvalidLogs_WhenFirstWorkAttendanceOtherThanIn()
        {
            var timeCardInit = new TimeCard()
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryStatus.Out, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.Status.Should().Be(TimeCardStatus.InvalidLogs);
        }

        [Fact]
        public void SetTimeCardStatus_ShouldSetInvalidLogs_WhenFirstBreakAttendanceOtherThanStart()
        {
            var timeCardInit = new TimeCard()
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                BreakAttendances = new BreakAttendanceCollection(new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryStatus.BreakEnd, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.Status.Should().Be(TimeCardStatus.InvalidLogs);
        }

        [Fact]
        public void SetTimeCardStatus_ShouldSetInvalidLogs_WhenFirstBreakAttendanceOtherThanStartButThereIsInOnWorkAttendances()
        {
            var timeCardInit = new TimeCard()
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryStatus.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                }),
                BreakAttendances = new BreakAttendanceCollection(new List<Attendance>
                {
                    new Attendance()
                    { Status = EntryStatus.BreakEnd, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.Status.Should().Be(TimeCardStatus.InvalidLogs);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        public void SetTimeCardStatus_ShouldNotChangeStatus_WhenStatusIsImmutable(int status)
        {
            var immutableStatus = WorkdayStatusSwitch(status);

            var timeCardInit = new TimeCard()
            {
                Status = (TimeCardStatus) immutableStatus,
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new()
                    { Status = EntryStatus.In, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                }),
                BreakAttendances = new BreakAttendanceCollection(new List<Attendance>
                {
                    new()
                    { Status = EntryStatus.BreakEnd, Timestamp = new DateTime(2020, 9, 29,  8, 2, 10) },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.Status.Should().Be(immutableStatus);
        }

        private static WorkdayStatus WorkdayStatusSwitch(int status)
        {
            return status switch
            {               
                0 => WorkdayStatus.VacationLeaveRequested,
                1 => WorkdayStatus.VacationLeaveApproved,
                2 => WorkdayStatus.PaidLeave,
                3 => WorkdayStatus.ParentalLeave,
                4 => WorkdayStatus.BusinessTripLeave,
                5 => WorkdayStatus.OvertimeLeave,
                6 => WorkdayStatus.SicknessLeave,
                _ => throw new Exception("Wrong input!!!")
            };
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenMultipleBreaksLastBreakStart()
        {
            var timeCardInit = new TimeCard
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  5, 59, 10)
                    },
                }),
                BreakAttendances = new BreakAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  9, 2, 10)
                    },
                    new()
                    {
                        Status = EntryStatus.BreakEnd,
                        Timestamp = new DateTime(2020, 9, 29,  10, 2, 10)
                    },
                    new()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  11, 2, 10)
                    },
                    new()
                    {
                        Status = EntryStatus.BreakEnd,
                        Timestamp = new DateTime(2020, 9, 29,  12, 2, 10)
                    },
                    new()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  13, 2, 10)
                    },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();
            
            timeCard.WorkDuration.Should().Be(4.Hours().And(50.Minutes()));
            timeCard.BreakDuration.Should().Be(2.Hours().And(0.Minutes()));
            timeCard.Status.Should().Be(TimeCardStatus.Break);
        }
        
        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenMultipleInOuts()
        {
            var timeCardInit = new TimeCard()
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  8, 2, 1),
                    },
                    new()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  10, 39, 54),
                    },
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  11, 51, 21),
                    },
                    new()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  12, 34, 34),
                    },
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  13, 21, 43),
                    },
                    new()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  14, 20, 21),
                    },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();
            
            timeCard.WorkDuration.Should().Be(4.Hours().And(20.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(0.Minutes()));
            timeCard.Status.Should().Be(TimeCardStatus.Undertime);
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenOneInOneBreakStart()
        {
            var timeCardInit = new TimeCard()
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  6, 21, 10),
                    },
                }),
                BreakAttendances = new BreakAttendanceCollection(new List<Attendance>
                {
                   new()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  10, 2, 10),
                    },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(3.Hours().And(40.Minutes()));
            timeCard.BreakDuration.Should().Be(2.Hours().And(50.Minutes()));
            timeCard.Status.Should().Be(TimeCardStatus.Break);
        }


        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenOneInOneOutOneBreak()
        {
            var timeCardInit = new TimeCard()
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  6, 21, 10),
                    },
                    new()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  12, 32, 10),
                    },
                }),
                BreakAttendances = new BreakAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  10, 2, 10),
                    },
                    new()
                    {
                        Status = EntryStatus.BreakEnd,
                        Timestamp = new DateTime(2020, 9, 29,  10, 32, 10),
                    },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(5.Hours().And(40.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(30.Minutes()));
            timeCard.Status.Should().Be(TimeCardStatus.Undertime);
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenMultipleInMultipleOutAndMultipleBreaks()
        {
            var timeCardInit = new TimeCard()
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  6, 21, 10),
                    },
                    new()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  12, 32, 10),
                    },
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  14, 32, 10),
                    },
                    new()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  16, 32, 10),
                    },
                }),
                BreakAttendances = new BreakAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  10, 2, 10),
                    },
                    new()
                    {
                        Status = EntryStatus.BreakEnd,
                        Timestamp = new DateTime(2020, 9, 29,  10, 32, 10),
                    },

                    new()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  11, 2, 10),
                    },
                    new()
                    {
                        Status = EntryStatus.BreakEnd,
                        Timestamp = new DateTime(2020, 9, 29,  11, 32, 10),
                    },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(7.Hours().And(10.Minutes()));
            timeCard.BreakDuration.Should().Be(1.Hours().And(00.Minutes()));
            timeCard.Status.Should().Be(TimeCardStatus.Undertime);
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldBeZero_WhenInAfterNow()
        {
            var timeCardInit = new TimeCard()
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  13, 59, 10),
                    },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(0.Hours().And(00.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(00.Minutes()));
            timeCard.Status.Should().Be(TimeCardStatus.NotAtWorkYet);
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldCalculateOnlyFirst_WhenOneInOneOutOneInAfterNow()
        {
            var timeCardInit = new TimeCard()
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  5, 2, 10)
                    },
                    new()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  11, 49, 10)
                    },
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  13, 59, 10),
                    },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(6.Hours().And(45.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(00.Minutes()));
            timeCard.Status.Should().Be(TimeCardStatus.Present);
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldCalculateTillNow_WhenOneInOneOutOneInBeforeNow()
        {
            var timeCardInit = new TimeCard()
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new Attendance()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  8, 2, 10)
                    },
                    new Attendance()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  11, 2, 10)
                    },
                    new Attendance()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  12, 2, 10)
                    },
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(3.Hours().And(50.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(00.Minutes()));
            timeCard.Status.Should().Be(TimeCardStatus.Present);
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldRoundToNearestCorrectly_WhenOneInOneBreak()
        {
            var timeCardInit = new TimeCard()
            {
                Day = new DateTime(2020, 9, 29, 0, 0, 0),
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>
                {
                    new Attendance()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  9, 24, 10)
                    },
                }),
                BreakAttendances = new BreakAttendanceCollection(new List<Attendance>
                {
                    new Attendance()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  10, 12, 10)
                    },
                    new Attendance()
                    {
                        Status = EntryStatus.BreakEnd,
                        Timestamp = new DateTime(2020, 9, 29,  11, 2, 10)
                    }
                }),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(timeCardInit)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(2.Hours().And(35.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(50.Minutes()));
            timeCard.Status.Should().Be(TimeCardStatus.Present);
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldReturnZero_WhenNoAttendances()
        {
            var workday = new TimeCard()
            {
                WorkAttendances = new WorkAttendanceCollection(new List<Attendance>()),
                BreakAttendances = new BreakAttendanceCollection(new List<Attendance>()),
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .SetTimeCardStatus()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(0.Hours().And(00.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(00.Minutes()));
        }
    }
}
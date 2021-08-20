namespace Beontime.Tests.TimeCalculator
{
    using System;
    using System.Collections.Generic;
    using Application.Common.Interfaces;
    using Domain.Entities;
    using Domain.Enums;
    using FluentAssertions;
    using FluentAssertions.Extensions;
    using Infrastructure.TimeCalculator;
    using Moq;
    using Xunit;

    public class TimeCardBuilderTests
    {
        private readonly IDateTimeService mockDateTimeService;
        private readonly DateTime now = new(2020, 9, 29, 12, 50, 10);

        public TimeCardBuilderTests()
        {
            mockDateTimeService = Mock.Of<IDateTimeService>();
            Mock.Get(mockDateTimeService)
                .Setup(dat => dat.Now)
                .Returns(now);
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenNoBreaks()
        {
            var workday = new WorkdayEntity
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29, 8, 2, 10),
                    },
                },
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(4.Hours().And(50.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(0.Minutes()));
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenMultipleBreaks()
        {
            var workday = new WorkdayEntity
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new()
                    {
                        Status = EntryStatus.In, 
                        Timestamp = new DateTime(2020, 9, 29, 7, 33, 10)
                    },
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
                },
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .GetTimeCard();
            
            timeCard.WorkDuration.Should().Be(2.Hours().And(15.Minutes()));
            timeCard.BreakDuration.Should().Be(3.Hours().And(0.Minutes()));
        }
        
        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenMultipleBreaksLastBreakStart()
        {
            var workday = new WorkdayEntity
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new ()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  5, 59, 10)
                    },
                    new ()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  9, 2, 10)
                    },
                    new ()
                    {
                        Status = EntryStatus.BreakEnd,
                        Timestamp = new DateTime(2020, 9, 29,  10, 2, 10)
                    },
                    new ()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  11, 2, 10)
                    },
                    new ()
                    {
                        Status = EntryStatus.BreakEnd,
                        Timestamp = new DateTime(2020, 9, 29,  12, 2, 10)
                    },
                    new ()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  13, 2, 10)
                    },
                },
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .GetTimeCard();
            
            timeCard.WorkDuration.Should().Be(4.Hours().And(50.Minutes()));
            timeCard.BreakDuration.Should().Be(2.Hours().And(0.Minutes()));
        }
        
        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenMultipleInOuts()
        {
            var workday = new WorkdayEntity()
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new ()
                    {
                        Status = EntryStatus.In, 
                        Timestamp = new DateTime(2020, 9, 29,  8, 2, 1),
                    },
                    new ()
                    { 
                        Status = EntryStatus.Out, 
                        Timestamp = new DateTime(2020, 9, 29,  10, 39, 54),
                    },
                    new ()
                    {
                        Status = EntryStatus.In, 
                        Timestamp = new DateTime(2020, 9, 29,  11, 51, 21),
                    },
                    new ()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  12, 34, 34),
                    },
                    new ()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  13, 21, 43),
                    },
                    new ()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  14, 20, 21),
                    },
                }
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .GetTimeCard();
            
            timeCard.WorkDuration.Should().Be(4.Hours().And(20.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(0.Minutes()));
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenOneInOneBreakStart()
        {
            var workday = new WorkdayEntity()
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new ()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  6, 21, 10),
                    },
                    new ()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  10, 2, 10),
                    },
                }
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(3.Hours().And(40.Minutes()));
            timeCard.BreakDuration.Should().Be(2.Hours().And(50.Minutes()));
        }


        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenOneInOneOutOneBreak()
        {
            var workday = new WorkdayEntity()
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new ()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  6, 21, 10),
                    },
                    new ()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  10, 2, 10),
                    },
                    new ()
                    {
                        Status = EntryStatus.BreakEnd,
                        Timestamp = new DateTime(2020, 9, 29,  10, 32, 10),
                    },
                    new ()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  12, 32, 10),
                    },
                }
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(5.Hours().And(40.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(30.Minutes()));
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldBeAsExpected_WhenMultipleInMultipleOutAndMultipleBreaks()
        {
            var workday = new WorkdayEntity()
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new ()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  6, 21, 10),
                    },
                    new ()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  10, 2, 10),
                    },
                    new ()
                    {
                        Status = EntryStatus.BreakEnd,
                        Timestamp = new DateTime(2020, 9, 29,  10, 32, 10),
                    },

                    new ()
                    {
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  11, 2, 10),
                    },
                    new ()
                    {
                        Status = EntryStatus.BreakEnd,
                        Timestamp = new DateTime(2020, 9, 29,  11, 32, 10),
                    },
                    new ()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  12, 32, 10),
                    },
                    new ()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  14, 32, 10),
                    },
                    new ()
                    {
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  16, 32, 10),
                    },
                }
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(7.Hours().And(10.Minutes()));
            timeCard.BreakDuration.Should().Be(1.Hours().And(00.Minutes()));
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldBeZero_WhenInAfterNow()
        {
            var workday = new WorkdayEntity()
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new ()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  13, 59, 10),
                    },
                }
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(0.Hours().And(00.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(00.Minutes()));
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldCalculateOnlyFirst_WhenOneInOneOutOneInAfterNow()
        {
            var workday = new WorkdayEntity()
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new AttendanceEntity()
                    { 
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  5, 2, 10)
                    },
                    new AttendanceEntity()
                    { 
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  11, 49, 10)
                    },
                    new ()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  13, 59, 10),
                    },
                }
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(6.Hours().And(45.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(00.Minutes()));
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldCalculateTillNow_WhenOneInOneOutOneInBeforeNow()
        {
            var workday = new WorkdayEntity()
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new AttendanceEntity()
                    { 
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  8, 2, 10)
                    },
                    new AttendanceEntity()
                    { 
                        Status = EntryStatus.Out,
                        Timestamp = new DateTime(2020, 9, 29,  11, 2, 10)
                    },
                    new AttendanceEntity()
                    { 
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  12, 2, 10)
                    }
                }
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(3.Hours().And(50.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(00.Minutes()));
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldRoundToNearestCorrectly_WhenOneInOneBreak()
        {
            var workday = new WorkdayEntity()
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new AttendanceEntity()
                    { 
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  9, 24, 10)
                    },
                    new AttendanceEntity()
                    { 
                        Status = EntryStatus.BreakStart,
                        Timestamp = new DateTime(2020, 9, 29,  10, 12, 10)
                    },
                    new AttendanceEntity()
                    { 
                        Status = EntryStatus.BreakEnd,
                        Timestamp = new DateTime(2020, 9, 29,  11, 2, 10)
                    }
                }
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(2.Hours().And(35.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(50.Minutes()));
        }

        [Fact]
        public void CalculatingTimeDuration_ShouldReturnZero_WhenNoAttendances()
        {
            var workday = new WorkdayEntity()
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>()
            };

            var timeCardBuilder = new TimeCardBuilder(mockDateTimeService);
            var timeCard = timeCardBuilder
                .BuildInitialData(workday)
                .CalculateBreakDuration()
                .CalculateWorkingDuration()
                .GetTimeCard();

            timeCard.WorkDuration.Should().Be(0.Hours().And(00.Minutes()));
            timeCard.BreakDuration.Should().Be(0.Hours().And(00.Minutes()));
        }
    }
}
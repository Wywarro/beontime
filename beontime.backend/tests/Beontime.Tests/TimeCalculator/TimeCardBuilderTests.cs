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
        private readonly DateTime now = new DateTime(2020, 9, 29, 12, 50, 10);

        public TimeCardBuilderTests()
        {
            mockDateTimeService = Mock.Of<IDateTimeService>();
            Mock.Get(mockDateTimeService)
                .Setup(dat => dat.Now)
                .Returns(now);
        }
        
        [Fact]
        public void CalculateWorkingDuration_ShouldCalculateDurationFromNow_WhenItsTodayAndOneInEntry()
        {
            var workday = new WorkdayEntity
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  8, 2, 10)
                    },
                }
            };

            var timeCardBuilder = new TimeCardBuilder(workday, mockDateTimeService);
            timeCardBuilder.CalculateWorkingDuration();
            var timeCard = timeCardBuilder.GetTimeCard();
            
            timeCard.WorkDuration.Should().Be(4.Hours().And(50.Minutes()));
        }
        
        [Fact]
        public void CalculateBreakDuration_ShouldBeTimeSpanZero_WhenNoBreaks()
        {
            var workday = new WorkdayEntity
            {
                Day = new DateTime(2020, 9, 29),
                Attendances = new List<AttendanceEntity>
                {
                    new()
                    {
                        Status = EntryStatus.In,
                        Timestamp = new DateTime(2020, 9, 29,  8, 2, 10)
                    },
                }
            };

            var timeCardBuilder = new TimeCardBuilder(workday, mockDateTimeService);
            timeCardBuilder.CalculateBreakDuration();
            var timeCard = timeCardBuilder.GetTimeCard();
            
            timeCard.BreakDuration.Should().Be(0.Hours()).And.Be(0.Minutes());
        }
    }
}
using Baseline.Dates;
using Beontime.Domain.Aggregates;
using Beontime.Domain.Events;
using Beontime.Infrastructure.TimeCards;
using BytesPack.Sterling.Takeoff.IntegrationTests.MartenTestHarness;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Beontime.IntegrationTests
{

    public class TimeCardsRepositoryTests : MartenTestHarness
    {
        private readonly Guid attendanceId1 = Guid.NewGuid();
        private readonly Guid attendanceId2 = Guid.NewGuid();
        private readonly Guid attendanceId3 = Guid.NewGuid();
        private readonly Guid attendanceId4 = Guid.NewGuid();

        public TimeCardsRepositoryTests(DefaultStoreFixture fixture) : base(fixture)
        {

        }

        [Fact]
        public async Task UpdateTimeCard_ShouldAddEventsToCreatedAttendances_WhenUpdatedWithEndOfWork()
        {
            var userId = Guid.NewGuid();

            var repo = new TimeCardsRepository(TheStore);

            var startDate = DateTime.Now;
            var start = new GettingWorkStarted(attendanceId1, userId, startDate);
            var id = await repo.CreateTimeCard(userId, start);

            var endDate = startDate.AddHours(2);

            var end = new EndOfWork(attendanceId2, userId, endDate);
            await repo.UpdateTimeCard(id, end);

            var timeCard = await repo.GetTimeCard(id);

            timeCard.WorkAttendances.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllTimeCards_ShouldAddEventsToCreatedAttendances_WhenUpdatedWithEndOfWork()
        {
            var userId = Guid.NewGuid();
            var timeCardId = Guid.NewGuid();

            var card = new TimeCard { Id = timeCardId, UserId = userId, };
            TheSession.Store(card);

            var date1 = DateTime.Now;
            var start1 = new GettingWorkStarted(attendanceId1, userId, date1);
            var end1 = new EndOfWork(attendanceId2, userId, date1.AddHours(1));
            var start2 = new GettingWorkStarted(attendanceId3, userId, date1.AddHours(2));
            var end2 = new EndOfWork(attendanceId4, userId, date1.AddHours(3));
            TheSession.Events.Append(timeCardId, start1, end1, start2, end2);
            TheSession.SaveChanges();

            var repo = new TimeCardsRepository(TheStore);
            var timeCards = new List<TimeCard>();
            await foreach (var timeCard in repo.GetAllTimeCards(userId, default))
            {
                timeCards.Add(timeCard);
            }

            timeCards.Should().HaveCount(1);
        }
    }
}

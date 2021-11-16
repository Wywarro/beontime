using Beontime.Application.Common.Interfaces;
using Beontime.Domain.Aggregates;
using Beontime.Domain.Events;
using Marten;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Beontime.Infrastructure.TimeCards
{

    public sealed class TimeCardsRepository : ITimeCardsRepository
    {
        private readonly IDocumentStore store;

        public TimeCardsRepository(IDocumentStore store)
        {
            this.store = store;
        }

        public async Task<IEnumerable<TimeCard>> GetAllTimeCards(
            Guid userId,
            CancellationToken token)
        {
            using var session = store.OpenSession();

            var timeCardIds = await session.Query<TimeCard>()
                .Where(t => t.UserId == userId)
                .Select(t => t.Id)
                .ToListAsync(token);

            var timeCards = new List<TimeCard>();
            foreach (var timeCardId in timeCardIds)
            {
                token.ThrowIfCancellationRequested();
                var timeCard = await session.Events
                    .AggregateStreamAsync<TimeCard>(timeCardId, token: token);
                if (timeCard is not null)
                {
                    timeCards.Add(timeCard);
                }
            }

            return timeCards;
        }

        public async Task<Guid> CreateTimeCard(Guid userId, params IEvent[] events)
        {
            using var session = store.OpenSession();

            var stream = session.Events.StartStream<TimeCard>(events);

            var timeCard = new TimeCard
            {
                Id = stream.Id,
                UserId = userId,
            };
            session.Store(timeCard);

            await session.SaveChangesAsync();

            return stream.Id;
        }

        public async Task UpdateTimeCard(Guid streamId, params IEvent[] events)
        {
            using var session = store.OpenSession();
            session.Events.Append(streamId, events);
            await session.SaveChangesAsync();
        }

        public async Task<TimeCard> GetTimeCard(Guid streamId)
        {
            using var session = store.OpenSession();
            var workday = await session.Events.AggregateStreamAsync<TimeCard>(streamId);

            return workday ?? new TimeCard();
        }
    }
}

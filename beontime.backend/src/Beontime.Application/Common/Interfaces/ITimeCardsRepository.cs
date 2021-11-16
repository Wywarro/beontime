using Beontime.Domain.Aggregates;
using Beontime.Domain.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Beontime.Application.Common.Interfaces
{
    public interface ITimeCardsRepository
    {
        Task<IEnumerable<TimeCard>> GetAllTimeCards(
            Guid userId,
            CancellationToken token);

        Task<Guid> CreateTimeCard(Guid userId, params IEvent[] events);
        Task UpdateTimeCard(Guid streamId, params IEvent[] events);
        Task<TimeCard> GetTimeCard(Guid streamId);
    }
}

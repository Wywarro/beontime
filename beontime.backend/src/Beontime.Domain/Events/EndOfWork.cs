using System;

namespace Beontime.Domain.Events
{

    public sealed record EndOfWork(
        Guid Id,
        Guid UserId,
        DateTime Timestamp) : IEvent
    {
    }
}

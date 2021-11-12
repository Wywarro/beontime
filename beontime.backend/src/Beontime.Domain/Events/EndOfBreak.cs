using System;

namespace Beontime.Domain.Events
{

    public sealed record EndOfBreak(
        Guid Id,
        Guid UserId,
        DateTime Timestamp) : IEvent
    {
    }
}

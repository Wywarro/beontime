using System;

namespace Beontime.Domain.Events
{

    public sealed record GettingBreakStarted(
        Guid Id,
        Guid UserId,
        DateTime Timestamp) : IEvent
    {
    }
}

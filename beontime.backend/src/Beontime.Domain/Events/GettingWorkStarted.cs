using System;

namespace Beontime.Domain.Events
{

    public sealed record GettingWorkStarted(
        Guid Id,
        Guid UserId,
        DateTime Timestamp) : IEvent
    {
    }
}

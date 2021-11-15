using System;

namespace Beontime.Domain.Events
{

    public sealed record TimeoffRequested(
        Guid Id,
        Guid UserId,
        DateTime Timestamp) : IEvent
    {
    }
}

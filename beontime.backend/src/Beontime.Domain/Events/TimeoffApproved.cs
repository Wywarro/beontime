using System;

namespace Beontime.Domain.Events
{

    public sealed record TimeoffApproved(
        Guid Id,
        Guid UserId,
        DateTime Timestamp) : IEvent
    {
    }
}

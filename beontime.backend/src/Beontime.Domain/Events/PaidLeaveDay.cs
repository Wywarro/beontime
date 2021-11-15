using System;

namespace Beontime.Domain.Events
{

    public sealed record PaidLeaveDay(
        Guid Id,
        Guid UserId,
        DateTime Timestamp) : IEvent
    {
    }
}

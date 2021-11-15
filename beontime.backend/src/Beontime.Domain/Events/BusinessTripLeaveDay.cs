using System;

namespace Beontime.Domain.Events
{

    public sealed record BusinessTripLeaveDay(
        Guid Id,
        Guid UserId,
        DateTime Timestamp) : IEvent
    {
    }
}

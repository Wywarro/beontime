using System;

namespace Beontime.Domain.Events
{

    public sealed record ParentalLeaveDay(
        Guid Id,
        Guid UserId,
        DateTime Timestamp) : IEvent
    {
    }
}

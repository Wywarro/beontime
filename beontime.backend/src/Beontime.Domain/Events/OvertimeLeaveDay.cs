using System;

namespace Beontime.Domain.Events
{

    public sealed record OvertimeLeaveDay(
        Guid Id,
        Guid UserId,
        DateTime Timestamp) : IEvent
    {
    }
}

using System;

namespace Beontime.Domain.Events
{

    public sealed record WorkingFromHome(
        Guid Id,
        Guid UserId,
        DateTime Timestamp) : IEvent
    {
    }
}

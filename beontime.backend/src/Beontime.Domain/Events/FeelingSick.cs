using System;

namespace Beontime.Domain.Events
{

    public sealed record FeelingSick(
        Guid Id,
        Guid UserId,
        DateTime Timestamp) : IEvent
    {
    }
}

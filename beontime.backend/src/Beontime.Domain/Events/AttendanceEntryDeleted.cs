using System;

namespace Beontime.Domain.Events
{

    public sealed record AttendanceEntryDeleted(
        Guid Id,
        Guid UserId,
        DateTime Timestamp
        )
    {
    }
}

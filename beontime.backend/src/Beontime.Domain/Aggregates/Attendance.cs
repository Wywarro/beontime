namespace Beontime.Domain.Aggregates
{
    using System;
    using Enums;

    public class Attendance
    {
        public Guid Id { get; set; }
        public EntryStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
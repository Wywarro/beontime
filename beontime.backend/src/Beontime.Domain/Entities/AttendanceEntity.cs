namespace Beontime.Domain.Entities
{
    using System;
    using Common;
    using Enums;

    public class AttendanceEntity : AuditableEntity
    {
        public int Id { get; set; }
        public int WorkdayId { get; set; }
        public EntryStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
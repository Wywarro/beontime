namespace Beontime.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Common;

    public sealed class WorkdayEntity : AuditableEntity
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public List<AttendanceEntity> Attendances { get; set; } = new();
    }
}
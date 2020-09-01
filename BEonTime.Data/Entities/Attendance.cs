using System;

namespace BEonTime.Data.Entities
{
    public class Attendance
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UserId { get; set; }
        public int WorkdayId { get; set; }
        public Workday Workday { get; set; }
        public EntryMode Status { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public enum EntryMode
    {
        In = 0,
        Out = 1,
        BreakEnd = 2,
        BreakStart = 3
    }
}

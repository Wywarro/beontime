using System;

namespace BIMonTime.Data.Entities
{
    public class Attendance
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int WorkdayId { get; set; }
        public Workday Workday { get; set; }
        public int Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

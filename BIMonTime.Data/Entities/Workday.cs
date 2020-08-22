using System;
using System.Collections.Generic;

namespace BIMonTime.Data.Entities
{
    public class Workday
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }
        public DateTime Datestamp { get; set; }
        public TimeSpan WorkDuration { get; set; }
        public TimeSpan BreakDuration { get; set; }
        public bool Verified { get; set; }
        public List<Attendance> Attendances { get; set; }
    }
}

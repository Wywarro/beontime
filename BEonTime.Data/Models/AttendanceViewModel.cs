using System;

namespace BEonTime.Data.Models
{
    public class AttendanceDetailModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public WorkdayDetailModel Workday { get; set; }
    }

    public class AttendanceCreateModel
    {
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
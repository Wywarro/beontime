using System;

namespace BIMonTime.Data.Models
{
    public class AttendanceDetailModel
    {
        public int Id { get; set; }
        public int WorkdayId { get; set; }
        public int Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BEonTime.Data.Models
{
    public class WorkdayListModel
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public DateTime Datestamp { get; set; }
        public TimeSpan WorkDuration { get; set; }
        public TimeSpan BreakDuration { get; set; }
        public bool Verified { get; set; }
        public int AttendancesCount { get; set; }
    }

    public class WorkdayDetailModel
    {
        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string Status { get; set; }
        public DateTime Datestamp { get; set; }
        public TimeSpan WorkDuration { get; set; }
        public TimeSpan BreakDuration { get; set; }
        public bool Verified { get; set; }
        public List<AttendanceDetailModel> Attendances { get; set; }
    }

    public class WorkdayUpdateModel
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public DateTime Datestamp { get; set; }
        public bool Verified { get; set; }
    }

    public class WorkdayCreateModel
    {
        [Required]
        public DateTime Datestamp { get; set; }
        [Required]
        public string Status { get; set; }
    }
}

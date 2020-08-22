using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BIMonTime.Data.Models
{
    public class WorkdayModel
    {
        [Required]
        public int Status { get; set; }
        [Required]
        public DateTime Datestamp { get; set; }
        [Required]
        public TimeSpan WorkDuration { get; set; }
        public TimeSpan BreakDuration { get; set; }
        [Required]
        public bool Verified { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace BEonTime.Data.Entities
{
    public class Workday
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UserId { get; set; }
        public WorkdayStatus Status { get; set; }
        public DateTime Datestamp { get; set; }
        public TimeSpan WorkDuration { get; set; }
        public TimeSpan BreakDuration { get; set; }
        public bool Verified { get; set; }
        public List<Attendance> Attendances { get; set; }
    }

    public enum WorkdayStatus
    {
        InvalidStatus = -1,

        Present = 0,
        ReadyToCalc = 1,

        NormalDay = 2,
        Overtime = 3,
        Undertime = 4,
        HomeOffice = 5,

        VacationLeaveRequested = 611,
        VacationLeaveApproved = 612,
        PaidLeave = 62,
        FamilyCare = 64,
        BusinessTrip = 65,
        OvertimeLeave = 67,
        SicknessLeave = 68,

        Break = 8,
        InvalidLogs = 9,
        UnexcusedAbsence = 10
    }
}

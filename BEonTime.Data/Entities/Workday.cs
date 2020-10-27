using BEonTime.Data.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace BEonTime.Data.Entities
{
    [BsonIgnoreExtraElements]
    [BsonCollection("workdays")]
    public class Workday : Document
    {
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
        ParentalLeave = 64,
        BusinessTripLeave = 65,
        OvertimeLeave = 67,
        SicknessLeave = 68,

        NotAtWorkYet = 7,
        Break = 8,
        InvalidLogs = 9,
        UnexcusedAbsence = 10
    }

    [BsonIgnoreExtraElements]
    public class Attendance
    {
        public DateTime UpdatedOn { get; set; }
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

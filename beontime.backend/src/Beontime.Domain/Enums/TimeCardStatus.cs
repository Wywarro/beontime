namespace Beontime.Domain.Enums
{
    public enum TimeCardStatus
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
        UnexcusedAbsence = 10,
    }
}
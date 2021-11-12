using System;

namespace Beontime.Domain.Enums
{
    public class WorkdayStatus : TimeCardStatus
    {
        static WorkdayStatus()
        {
            HomeOffice = new WorkdayStatus(5);
            VacationLeaveRequested = new WorkdayStatus(611);
            VacationLeaveApproved = new WorkdayStatus(612);
            PaidLeave = new WorkdayStatus(62);
            ParentalLeave = new WorkdayStatus(64);
            BusinessTripLeave = new WorkdayStatus(65);
            OvertimeLeave = new WorkdayStatus(67);
            SicknessLeave = new WorkdayStatus(68);
        }

        private WorkdayStatus(int value) : base(value)
        { }

        public static WorkdayStatus HomeOffice { get; }
        public static WorkdayStatus VacationLeaveRequested { get; }
        public static WorkdayStatus VacationLeaveApproved { get; }
        public static WorkdayStatus PaidLeave { get; }
        public static WorkdayStatus ParentalLeave { get; }
        public static WorkdayStatus BusinessTripLeave { get; }
        public static WorkdayStatus OvertimeLeave { get; }
        public static WorkdayStatus SicknessLeave { get; }

        public override string ToString()
        {
            var workdayStatus = Value switch
            {
                5 => nameof(HomeOffice),
                611 => nameof(VacationLeaveRequested),
                612 => nameof(VacationLeaveApproved),
                62 => nameof(PaidLeave),
                64 => nameof(ParentalLeave),
                65 => nameof(BusinessTripLeave),
                67 => nameof(OvertimeLeave),
                68 => nameof(SicknessLeave),
                _ => null,
            };

            if (workdayStatus is not null)
            {
                return workdayStatus;
            }

            return base.ToString();
        }
    }
}

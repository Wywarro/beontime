using BEonTime.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BEonTime.Services.TimeCalculator
{
    public static class WorkdayExtensions
    {
        public static bool IsStatusMutable(this Workday workday)
        {
            WorkdayStatus[] mutableStatuses = new WorkdayStatus[]
            {
                WorkdayStatus.InvalidStatus,
                WorkdayStatus.Present,
                WorkdayStatus.ReadyToCalc,
                WorkdayStatus.NormalDay,
                WorkdayStatus.Overtime,
                WorkdayStatus.Undertime,
                WorkdayStatus.Break,
                WorkdayStatus.InvalidLogs,
                WorkdayStatus.UnexcusedAbsence,
            };
            return mutableStatuses.Any(immStat => immStat == workday.Status);
        }

        public static bool IsStatusImmutable(this Workday workday)
        {
            WorkdayStatus[] immutableStatuses = new WorkdayStatus[]
            {
                WorkdayStatus.HomeOffice,
                WorkdayStatus.VacationLeaveRequested,
                WorkdayStatus.VacationLeaveApproved,
                WorkdayStatus.PaidLeave,
                WorkdayStatus.ParentalLeave,
                WorkdayStatus.BusinessTripLeave,
                WorkdayStatus.OvertimeLeave,
                WorkdayStatus.SicknessLeave,
            };
            return immutableStatuses.Any(immStat => immStat == workday.Status);
        }
    }
}

using BEonTime.Data.Entities;
using BEonTime.Services.DateTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using static BEonTime.Services.TimeCalculator.AttendanceValidatorFactory;
using static BEonTime.Services.TimeCalculator.WorkdayStatusValidator;

namespace BEonTime.Services.TimeCalculator
{
    public interface IAttendanceTimeCalculator
    {
        void GetWorkingTime(Workday workday);
    }

    public class AttendanceTimeCalculator : IAttendanceTimeCalculator
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private Workday _workday;

        private readonly DateTime now;

        public AttendanceTimeCalculator(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
            now = dateTimeProvider.GetDateTimeNow();
        }

        public void GetWorkingTime(Workday workday)
        {
            _workday = workday;
            _workday.Attendances.Sort((x, y) => x.Timestamp.CompareTo(y.Timestamp));
            if (_workday.IsStatusMutable())
                SetStatus();

            if (_workday.MakeCalculations())
                CalculateWorkingTime();
                CalculateBreak();

            _workday.BreakDuration = TimeSpan.FromSeconds(60);
        }

        private void SetStatus()
        {
            StatusChainHandler handler = GenerateStatus(_workday, now);
            var result = handler.Handle();
            _workday.Status = (WorkdayStatus) result;
        }

        private void CalculateWorkingTime()
        {
            List<Attendance> attendances = _workday.Attendances;
            var todayLastAtt = new Attendance { Timestamp = now, Status = EntryMode.Out };

            var inOutPairs = attendances.Where(att => att.Status == EntryMode.In)
                .Select((att, i) => new Tuple<Attendance, Attendance>(
                    att,
                    attendances
                        .SkipWhile(x => x != att)
                        .FirstOrDefault(att => att.Status == EntryMode.Out) ?? todayLastAtt)).ToList();

            foreach (var inOut in inOutPairs)
            {
                TimeSpan workDuration = inOut.Item2.Timestamp - inOut.Item1.Timestamp;
                _workday.WorkDuration += workDuration;
            }
        }

        private void CalculateBreak()
        {
            List<Attendance> attendances = _workday.Attendances;
            var todayLastAtt = new Attendance { Timestamp = now, Status = EntryMode.BreakEnd };

            var inOutPairs = attendances.Where(att => att.Status == EntryMode.BreakStart)
                .Select(att => new Tuple<Attendance, Attendance>(
                    att,
                    attendances
                        .SkipWhile(x => x != att)
                        .FirstOrDefault(att => att.Status == EntryMode.BreakEnd) ?? todayLastAtt)).ToList();

            foreach (var inOut in inOutPairs)
            {
                TimeSpan breakDuration = inOut.Item2.Timestamp - inOut.Item1.Timestamp;
                _workday.BreakDuration += breakDuration;
            }

            _workday.WorkDuration -= _workday.BreakDuration;
        }
    }
}

using BEonTime.Data.Entities;
using BEonTime.Services.DateTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using static BEonTime.Services.TimeCalculator.WorkdayStatusValidator;

namespace BEonTime.Services.TimeCalculator
{
    public interface IAttendanceTimeCalculator
    {
        void GetWorkingTime(Workday workday);
    }

    public class AttendanceTimeCalculator : IAttendanceTimeCalculator
    {
        private Workday _workday;
        private readonly DateTime now;
        private readonly int NearestMinutes = 5;

        public AttendanceTimeCalculator(IDateTimeProvider dateTimeProvider)
        {
            now = dateTimeProvider.GetDateTimeNow();
        }

        public void GetWorkingTime(Workday workday)
        {
            _workday = workday;
            _workday.Attendances.Sort((x, y) => x.Timestamp.CompareTo(y.Timestamp));
            if (_workday.IsStatusMutable())
                SetStatus();

            if (_workday.MakeCalculations())
            {
                _workday.BreakDuration = CalculateWorkingTime(EntryMode.BreakStart, EntryMode.BreakEnd,
                    RevertToPresent());
                _workday.WorkDuration = CalculateWorkingTime(EntryMode.In, EntryMode.Out,
                    RevertToNotAtWorkYet()) - _workday.BreakDuration;
            }
        }

        private RevertStatus RevertToPresent() => () => _workday.Status = WorkdayStatus.Present;
        private RevertStatus RevertToNotAtWorkYet() => () => _workday.Status = WorkdayStatus.NotAtWorkYet;

        private void SetStatus()
        {
            StatusChainHandler handler = GenerateStatus(_workday, now);
            var result = handler.Handle();
            _workday.Status = (WorkdayStatus) result;
        }

        public delegate void RevertStatus();

        private TimeSpan CalculateWorkingTime(EntryMode entryStart, EntryMode entryEnd, RevertStatus revertStatus)
        {
            List<Attendance> attendances = _workday.Attendances;
            var todayLastAtt = new Attendance { Timestamp = now, Status = entryEnd };

            var startEndPairs = attendances.Where(att => att.Status == entryStart)
                .Select(att => new Tuple<Attendance, Attendance>(
                    att,
                    attendances
                        .SkipWhile(x => x != att)
                        .FirstOrDefault(att => att.Status == entryEnd) ?? todayLastAtt)).ToList();

            TimeSpan duration = TimeSpan.Zero;
            foreach (var startEnd in startEndPairs)
            {
                TimeSpan workDuration = startEnd.Item2.Timestamp - startEnd.Item1.Timestamp;
                if (workDuration.Ticks >= 0)
                    duration += workDuration;
                else
                    revertStatus();
            }

            return duration.RoundToNearestMinutes(NearestMinutes);
        }
    }
}

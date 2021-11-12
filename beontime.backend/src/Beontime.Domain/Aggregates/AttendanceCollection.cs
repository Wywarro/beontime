using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Beontime.Domain.Aggregates
{

    public abstract class AttendanceCollection : IEnumerable<Attendance>
    {
        public int Count => attendances.Count;
        protected readonly List<Attendance> attendances = new();

        public AttendanceCollection()
        {

        }

        public AttendanceCollection(IEnumerable<Attendance> attendances)
        {
            this.attendances = attendances.ToList();
        }

        public void Add(Attendance attendance)
        {
            attendances.Add(attendance);
        }

        public void AddRange(IEnumerable<Attendance> newAttendances)
        {
            attendances.AddRange(newAttendances);
        }

        public bool AreAttendancesInvalid()
        {
            if (IsFirstAttendanceStatusInvalid()) return true;

            for (int i = 0; i < attendances.Count - 1; i++)
            {
                var attendance = attendances[i];
                var nextAttendance = attendances
                    .SkipWhile(x => x != attendance)
                    .Skip(1)
                    .FirstOrDefault() ?? new Attendance();

                bool nextAttInvalid = attendance.Status == nextAttendance.Status;
                if (nextAttInvalid)
                {
                    return true;
                }
            }

            return false;
        }

        protected abstract bool IsFirstAttendanceStatusInvalid();

        public IEnumerator<Attendance> GetEnumerator()
        {
            return attendances.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) attendances).GetEnumerator();
        }
    }
}

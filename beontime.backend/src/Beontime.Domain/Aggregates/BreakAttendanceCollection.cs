using Beontime.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Beontime.Domain.Aggregates
{

    public sealed class BreakAttendanceCollection : AttendanceCollection
    {
        public BreakAttendanceCollection()
        {

        }

        public BreakAttendanceCollection(IEnumerable<Attendance> attendances) : base(attendances)
        {

        }

        public int BreakStartCount => attendances
            .Where(att => att.Status == EntryStatus.BreakStart)
            .Count();
        public int BreakEndCount => attendances
            .Where(att => att.Status == EntryStatus.BreakEnd)
            .Count();

        protected override bool IsFirstAttendanceStatusInvalid()
        {
            var firstOne = attendances.FirstOrDefault();
            if (firstOne is null)
            {
                return false;
            }

            return firstOne.Status != EntryStatus.BreakStart;
        }
    }
}

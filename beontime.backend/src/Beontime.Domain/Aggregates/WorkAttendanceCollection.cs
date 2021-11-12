using Beontime.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Beontime.Domain.Aggregates
{

    public sealed class WorkAttendanceCollection : AttendanceCollection
    {
        public WorkAttendanceCollection()
        {

        }

        public WorkAttendanceCollection(IEnumerable<Attendance> attendances) : base(attendances)
        {

        }

        public int InCount => attendances
            .Where(att => att.Status == EntryStatus.In)
            .Count();
        public int OutCount => attendances
            .Where(att => att.Status == EntryStatus.Out)
            .Count();

        protected override bool IsFirstAttendanceStatusInvalid()
        {
            var firstOne = attendances.FirstOrDefault();
            if (firstOne is null)
            {
                return false;
            }

            return firstOne.Status != EntryStatus.In;
        }
    }
}

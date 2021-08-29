using Beontime.Domain.Entities;
using Beontime.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Beontime.Infrastructure.TimeCalculator.AttendanceValidators
{
    internal static class AttendanceStatusValidator
    {
        public static bool AreAttendancesInvalid(this IEnumerable<AttendanceEntity> attendances)
        {
            if (attendances.IsFirstAttendanceStatusInvalid())
            {
                return true;
            }

            for (int i = 0; i < attendances.Count() - 1; i++)
            {
                var attendance = attendances.ElementAt(i);

                var nextAttendance = attendances
                    .SkipWhile(x => x != attendance)
                    .Skip(1)
                    .FirstOrDefault() ?? new AttendanceEntity();

                bool nextAttInvalid = attendance.IsNextAttendanceStatusValid(nextAttendance);

                if (nextAttInvalid)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsFirstAttendanceStatusInvalid(this IEnumerable<AttendanceEntity> attendances)
        {
            return attendances.First().Status != EntryStatus.In;
        }

        private static bool IsNextAttendanceStatusValid(
            this AttendanceEntity attendance,
            AttendanceEntity nextAttendance)
        {
            var allowedModes = GetAllowedModesForNextAttendance(attendance);

            bool nextStatusInvalid = allowedModes.All(mod => mod != nextAttendance.Status);

            return nextStatusInvalid;
        }

        private static List<EntryStatus> GetAllowedModesForNextAttendance(AttendanceEntity att)
        {
            return att.Status switch
            {
                EntryStatus.In => new List<EntryStatus> { EntryStatus.BreakStart, EntryStatus.Out },
                EntryStatus.BreakStart => new List<EntryStatus> { EntryStatus.BreakEnd },
                EntryStatus.BreakEnd => new List<EntryStatus> { EntryStatus.BreakStart, EntryStatus.Out },
                EntryStatus.Out => new List<EntryStatus> { EntryStatus.In },

                _ => new List<EntryStatus>(),
            };
        }
    }
}

using BEonTime.Data;
using BEonTime.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEonTime.Services.Repositories
{
    public interface IAttendanceRepository
    {
        Task<Attendance> CreateAttendance(Attendance attendance);
        Task<IEnumerable<Attendance>> GetAllAttendances(string userId);
        Task<Attendance> GetAttendance(int attendanceId);
        Task<Attendance> GetAttendance(DateTime datestamp, string userId);
        Task UpdateAttendance(Attendance attendance);
        Task DeleteAttendance(int attendanceId);
    }

    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext context;
        public AttendanceRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Attendance> CreateAttendance(Attendance attendance)
        {
            context.Add(attendance);
            await context.SaveChangesAsync();
            return attendance;
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendances(string userId)
        {
            return await context.Attendances.Where(workday =>
                workday.UserId == userId).ToListAsync();
        }

        public async Task<Attendance> GetAttendance(int attendanceId)
        {
            return await context.Attendances.FindAsync(attendanceId);
        }

        public async Task<Attendance> GetAttendance(DateTime timestamp, string userId)
        {
            return await context.Attendances.SingleOrDefaultAsync(attendance =>
                attendance.Timestamp == timestamp &&
                attendance.UserId == userId);
        }

        public async Task UpdateAttendance(Attendance attendance)
        {
            context.Attendances.Update(attendance);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAttendance(int attendanceId)
        {
            var attendanceToDelete = await context.Attendances.FindAsync(attendanceId);
            if (attendanceToDelete == null)
                throw new InvalidOperationException("Workday does not exist!");

            context.Remove(attendanceToDelete);
            await context.SaveChangesAsync();
        }
    }
}

using BIMonTime.Data;
using BIMonTime.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BIMonTime.Services.Repositories
{
    public class WorkdayRepository : IWorkdayRepository
    {
        private readonly ApplicationDbContext context;
        public WorkdayRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Workday> CreateWorkday(Workday workday)
        {
            context.Add(workday);
            await context.SaveChangesAsync();
            return workday;
        }

        public async Task<IEnumerable<Workday>> GetAllWorkdays()
        {
            return await context.Workdays.ToListAsync();
        }

        public async Task<Workday> GetWorkday(int workdayId)
        {
            return await context.Workdays.FindAsync(workdayId);
        }

        public async Task<Workday> GetWorkday(DateTime datestamp, string userId)
        {
            return await context.Workdays.FindAsync(datestamp, userId);
        }

        public async Task UpdateWorkday(Workday workday)
        {
            context.Workdays.Update(workday);
            await context.SaveChangesAsync();
        }

        public async Task DeleteWorkday(int id)
        {
            var workdayToDelete = await context.Workdays.FindAsync(id);
            if (workdayToDelete == null) throw new InvalidOperationException("Workday does not exist!");

            context.Remove(workdayToDelete);
            await context.SaveChangesAsync();
        }
    }
}

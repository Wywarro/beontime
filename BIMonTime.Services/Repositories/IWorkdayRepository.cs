using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BIMonTime.Data.Entities;

namespace BIMonTime.Services.Repositories
{
    public interface IWorkdayRepository
    {
        Task<Workday> CreateWorkday(Workday entity);
        Task<IEnumerable<Workday>> GetAllWorkdays();
        Task<Workday> GetWorkday(int id);
        Task<Workday> GetWorkday(DateTime datestamp, string userId);
        Task UpdateWorkday(Workday entity);
        Task DeleteWorkday(int id);
    }
}

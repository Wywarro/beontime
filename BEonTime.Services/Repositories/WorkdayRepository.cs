using BEonTime.Data;
using BEonTime.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BEonTime.Services.Repositories
{
    public interface IWorkdayRepository
    {
        Task InsertOneAsync(Workday workday);
        Task<List<Workday>> FilterByAsync(
            Expression<Func<Workday, bool>> filterExpression);

        Task<Workday> FindOneAsync(
            Expression<Func<Workday, bool>> filterExpression);
        Task<Workday> FindByIdAsync(string id);
        Task ReplaceOneAsync(Workday workday);
        Task DeleteByIdAsync(string id);
    }

    public class WorkdayRepository : GenericRepository<Workday>, IWorkdayRepository
    {
        public WorkdayRepository(IAppDbContext context) : base(context)
        { }

    }
}

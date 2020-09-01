using BEonTime.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BEonTime.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public virtual DbSet<Workday> Workdays { get; set; }
        public virtual DbSet<Attendance> Attendances { get; set; }
    }
}

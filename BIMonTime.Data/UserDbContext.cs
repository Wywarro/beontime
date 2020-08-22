using BIMonTime.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BIMonTime.Data
{
    public class UserDbContext : IdentityDbContext<BeOnTimeUser>
    {
        public UserDbContext()
        { }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        { }
    }
}

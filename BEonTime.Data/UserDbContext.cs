using BEonTime.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BEonTime.Data
{
    public class UserDbContext : IdentityDbContext<BEonTimeUser>
    {
        public UserDbContext()
        { }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        { }
    }
}

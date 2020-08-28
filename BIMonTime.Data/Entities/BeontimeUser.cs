using Microsoft.AspNetCore.Identity;
using System;

namespace BIMonTime.Data.Entities
{
    public class BeOnTimeUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime CareerStarted { get; set; }
    }
}

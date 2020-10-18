using Microsoft.AspNetCore.Identity;
using System;

namespace BEonTime.Data.Entities
{
    public class BEonTimeUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime CareerStarted { get; set; }
        public DeviceUser DeviceUser { get; set; }
    }
}

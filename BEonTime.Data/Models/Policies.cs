using Microsoft.AspNetCore.Authorization;

namespace BEonTime.Data.Models
{
    public class Policies
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Employee = "Employee";

        public static AuthorizationPolicy AdminPolicy() =>
            new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .RequireRole(Admin).Build();

        public static AuthorizationPolicy ManagerPolicy() =>
            new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .RequireRole(Manager).Build();

        public static AuthorizationPolicy EmployeePolicy() =>
            new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .RequireRole(Employee).Build();
    }
}

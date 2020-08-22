﻿using Microsoft.AspNetCore.Authorization;

namespace BIMonTime.Data.Models
{
    public class Policies
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string User = "User";

        public static AuthorizationPolicy AdminPolicy() =>
            new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .RequireRole(Admin).Build();

        public static AuthorizationPolicy ManagerPolicy() =>
            new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .RequireRole(Manager).Build();

        public static AuthorizationPolicy UserPolicy() => 
            new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .RequireRole(User).Build();
    }
}

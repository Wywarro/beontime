using Beontime.Infrastructure.JwtService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Beontime.WebApi.ApplicationRegistrations
{

    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddSterlingAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtConfiguration = configuration.GetSection(JwtConfigSettings.SectionName)
                .Get<JwtConfigSettings>();

            services.AddAuthorization(config =>
            {
                config.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                config.AddPolicy(Policies.Manager, Policies.ManagerPolicy());
                config.AddPolicy(Policies.Employee, Policies.EmployeePolicy());
            });

            services
                .AddAuthentication(c =>
                {
                    c.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtConfiguration.Issuer,
                        ValidAudience = jwtConfiguration.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtConfiguration.SecurityKey)),
                        ClockSkew = TimeSpan.Zero,
                    };
                });

            return services;
        }
    }
}

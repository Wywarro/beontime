using AutoMapper;
using BEonTime.Data;
using BEonTime.Data.Entities;
using BEonTime.Data.Models;
using BEonTime.Services.Auth;
using BEonTime.Services.DateTimeProvider;
using BEonTime.Services.EnvironmentVariables;
using BEonTime.Services.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace BEonTime.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateFormatString = "dd'-'MM'-'yyyy' 'HH':'mm";
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.EnableDetailedErrors();
                options.UseNpgsql(Configuration.GetConnectionString("BEonTime.dev"));
            });
            services.AddDbContext<UserDbContext>(options =>
            {
                options.EnableDetailedErrors();
                options.UseNpgsql(Configuration.GetConnectionString("BEonTime.dev"));
            });

            services.AddIdentityCore<BEonTimeUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<UserDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication("oauth")
                .AddJwtBearer("oauth", config =>
                {
                    config.RequireHttpsMetadata = false;
                    config.SaveToken = true;
                    config.ClaimsIssuer = Configuration["Jwt:Issuer"];
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(EnvVarProvider.SecretKey)),

                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],

                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(config =>
            {
                config.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                config.AddPolicy(Policies.Manager, Policies.ManagerPolicy());
                config.AddPolicy(Policies.Employee, Policies.EmployeePolicy());
            });

            services.AddTransient<IWorkdayRepository, WorkdayRepository>();
            services.AddTransient<IAttendanceRepository, AttendanceRepository>();

            services.AddSingleton<IJwtFactory, JwtFactory>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseCors(builder => builder
                .WithOrigins(
                    "http://localhost:8080")
            );

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

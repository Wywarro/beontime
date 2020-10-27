using AutoMapper;
using BEonTime.Data;
using BEonTime.Data.Models;
using BEonTime.Services.Auth;
using BEonTime.Services.DateTimeProvider;
using BEonTime.Services.EmailSender;
using BEonTime.Services.EnvironmentVariables;
using BEonTime.Services.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization.Conventions;
using Newtonsoft.Json;
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
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                options.SerializerSettings.DateFormatString = "dd'-'MM'-'yyyy' 'HH':'mm";
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
            services.AddRazorPages().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                options.SerializerSettings.DateFormatString = "dd'-'MM'-'yyyy' 'HH':'mm";
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            ConventionRegistry.Register(
                "Camel Case", 
                new ConventionPack { new CamelCaseElementNameConvention() }, 
                _ => true);

            services.Configure<MongoDBOptions>(options =>
            {
                var mongoOptions = new MongoDBOptions();
                Configuration.Bind("MongoDBOptions", mongoOptions);

                string passDb = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
                string userDb = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
                string bareConnectionString = mongoOptions.ConnectionString;
                string connectionString = bareConnectionString
                    .Replace("<password>", passDb)
                    .Replace("<username>", userDb);

                options.ConnectionString = connectionString;
                options.Database = mongoOptions.Database;
                options.User = mongoOptions.User;
                options.Role = mongoOptions.Role;
            });

            services.Configure<EmailSenderMetadata>(metadata =>
            {
                string emailPass = Environment.GetEnvironmentVariable("EMAIL_SENDER_PASSWORD");
                var emailSender = new EmailSenderMetadata
                { Password = emailPass };
                Configuration.Bind("EmailSender", emailSender);

                metadata.Password = emailSender.Password;
                metadata.Port = emailSender.Port;
                metadata.SmtpServer = emailSender.SmtpServer;
                metadata.Username = emailSender.Username;
            });

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

            services.AddLogging(options =>
            {
                options.AddDebug();
                options.AddConsole();
            });

            services.AddTransient<IAppDbContext, AppDbContext>();
            services.AddTransient<IWorkdayRepository, WorkdayRepository>();

            services.AddSingleton<IJwtFactory, JwtFactory>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            services.AddSingleton<IEmailSender, EmailSender>();

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
                endpoints.MapRazorPages();
            });
        }
    }
}

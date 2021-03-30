using System;
using AspNetCore.Identity.Mongo.Model;
using AspNetCore.Identity.Mongo;
using BEonTime.Data;
using BEonTime.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using BEonTime.Data.Models;
using Microsoft.Extensions.Configuration;

[assembly: HostingStartup(typeof(BEonTime.Web.Areas.Identity.IdentityHostingStartup))]
namespace BEonTime.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {

                services.AddIdentityMongoDbProvider<BEonTimeUser, MongoRole>(identityOptions =>
                {
                    // Password settings.
                    identityOptions.Password.RequiredLength = 6;
                    identityOptions.Password.RequireLowercase = true;
                    identityOptions.Password.RequireUppercase = true;
                    identityOptions.Password.RequireNonAlphanumeric = false;
                    identityOptions.Password.RequireDigit = true;

                    // Lockout settings.
                    identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    identityOptions.Lockout.MaxFailedAccessAttempts = 5;
                    identityOptions.Lockout.AllowedForNewUsers = true;

                    // User settings.
                    identityOptions.User.AllowedUserNameCharacters =
                      "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    identityOptions.User.RequireUniqueEmail = true;
                }, mongoIdentityOptions => {
                    var mongoOptions = new MongoDBOptions();
                    context.Configuration.Bind("MongoDBOptions", mongoOptions);

                    string passDb = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
                    string userDb = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
                    string bareConnectionString = mongoOptions.ConnectionString;
                    string connectionString = bareConnectionString
                        .Replace("<password>", passDb)
                        .Replace("<username>", userDb);

                    mongoIdentityOptions.ConnectionString = $"{connectionString}/{mongoOptions.Database}";
                    mongoIdentityOptions.UsersCollection = mongoOptions.User.CollectionName;
                    mongoIdentityOptions.RolesCollection = mongoOptions.Role.CollectionName;
                }).AddDefaultUI(); //.AddDefaultUI() to temporary remove error when no EmailSender provided, see https://stackoverflow.com/questions/52089864/

                // This is required to ensure server can identify user after login
                services.ConfigureApplicationCookie(options =>
                {
                    // Cookie settings
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                    options.LoginPath = "/Identity/Account/Login";
                    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                    options.SlidingExpiration = true;
                });
            });
        }
    }
}
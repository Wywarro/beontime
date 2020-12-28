using AutoMapper;
using BEonTime.Data;
using BEonTime.Data.Models;
using BEonTime.Services.Auth;
using BEonTime.Services.DateTimeProvider;
using BEonTime.Services.EmailSender;
using BEonTime.Services.EnvironmentVariables;
using BEonTime.Services.Repositories;
using BEonTime.Web.Middlewares;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Newtonsoft.Json;
using Serilog;
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

        public void SetJsonOptions(MvcNewtonsoftJsonOptions options) 
        {
            options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            options.SerializerSettings.DateFormatString = "dd'-'MM'-'yyyy' 'HH':'mm";
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(SetJsonOptions);
            services.AddRazorPages().AddNewtonsoftJson(SetJsonOptions);

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = @"ClientApp/dist";
            });

            services.AddCors(options =>
            {
                options.AddPolicy("VueCorsPolicy", builder =>
                {
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins("https://localhost:5001");
                });
            });

            var camelCaseConvetionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("Camel Case", camelCaseConvetionPack, _ => true);

            services.Configure<MongoDBOptions>(options =>
            {
                var mongoOptions = CreateMongoDBOptions(Configuration);

                options.ConnectionString = mongoOptions.ConnectionString;
                options.Database = mongoOptions.Database;
                options.User = mongoOptions.User;
                options.Role = mongoOptions.Role;
            });

            var mongoOptions = CreateMongoDBOptions(Configuration);
            var mongoClient = new MongoClient(mongoOptions.ConnectionString);
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMongoStorage(mongoClient, mongoOptions.Database, new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    Prefix = "hangfire.mongo",
                    CheckConnection = true
                })
            );

            services.AddHangfireServer(serverOptions =>
            {
                serverOptions.ServerName = "Hangfire.Mongo beontime server";
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

        public static MongoDBOptions CreateMongoDBOptions(IConfiguration configuration)
        {
            var mongoOptions = new MongoDBOptions();
            configuration.Bind("MongoDBOptions", mongoOptions);

            string passDb = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
            string userDb = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
            string bareConnectionString = mongoOptions.ConnectionString;
            string connectionString = bareConnectionString
                .Replace("<password>", passDb)
                .Replace("<username>", userDb);

            mongoOptions.ConnectionString = connectionString;

            return mongoOptions;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.UseHttpsRedirection();
            app.UseCookiePolicy();

            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthentication();

            app.UseCors("VueCorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
            app.UseSpaStaticFiles();

            app.UseSpa(configuration: builder =>
            {
                if (env.IsDevelopment())
                {
                    //builder.UseProxyToSpaDevelopmentServer("http://localhost:8080");
                }
                else
                {
                    builder.Options.SourcePath = @"ClientApp/dist";
                }
            });
        }
    }
}

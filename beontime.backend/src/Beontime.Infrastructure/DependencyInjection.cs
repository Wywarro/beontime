using Beontime.Application.Common.Interfaces;
using Beontime.Infrastructure.EmailSender;
using Beontime.Infrastructure.JwtService;
using Beontime.Infrastructure.Services;
using Beontime.Infrastructure.TimeCards;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Postgresql;

namespace Beontime.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<IDateTimeService, DateTimeService>();

            services.AddOptions<JwtConfigSettings>()
                .Bind(configuration.GetSection(JwtConfigSettings.SectionName));
            services.AddSingleton<IJwtFactory, JwtFactory>();

            services.AddOptions<EmailConfigSettings>()
                .Bind(configuration.GetSection(EmailConfigSettings.SectionName));
            services.AddSingleton<IEmailService, EmailService>();

            services.AddTransient<ITimeCardsRepository, TimeCardsRepository>();

            var postgreSqlConnectionString = configuration.GetConnectionString("POSTGRESQL");
            services.AddMarten(options =>
            {
                options.AutoCreateSchemaObjects = AutoCreate.All;
                options.Connection(postgreSqlConnectionString);
            }).InitializeStore();

            return services;
        }
    }
}
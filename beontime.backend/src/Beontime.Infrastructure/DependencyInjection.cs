using Beontime.Application.Common.Interfaces;
using Beontime.Infrastructure.EmailSender;
using Beontime.Infrastructure.JwtService;
using Beontime.Infrastructure.MartenConfig;
using Beontime.Infrastructure.Services;
using Beontime.Infrastructure.TimeCards;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beontime.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<JwtConfigSettings>()
                .Bind(configuration.GetSection(JwtConfigSettings.SectionName));
            services.AddSingleton<IJwtFactory, JwtFactory>();

            services.AddOptions<EmailConfigSettings>()
                .Bind(configuration.GetSection(EmailConfigSettings.SectionName));
            services.AddSingleton<IEmailService, EmailService>();

            services.AddTransient<ITimeCardsRepository, TimeCardsRepository>();

            services.AddTransient<IPasswordGenerator, PasswordGenerator>();
            services.AddTransient<IDateTimeService, DateTimeService>();

            services.AddBeontimeMartenDb(configuration);

            return services;
        }
    }
}
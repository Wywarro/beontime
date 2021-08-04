namespace Beontime.Infrastructure
{
    using Application.Common.Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Services;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IDateTimeService, DateTimeService>();
            
            return services;
        }
    }
}
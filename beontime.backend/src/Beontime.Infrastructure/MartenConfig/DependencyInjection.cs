using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Postgresql;

namespace Beontime.Infrastructure.MartenConfig
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddBeontimeMartenDb(
            this IServiceCollection services,
            IConfiguration configuration)
        {
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

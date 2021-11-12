namespace BytesPack.Sterling.Takeoff.IntegrationTests.MartenTestHarness
{
    using Marten;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.IO;

    public class ConnectionSource : ConnectionFactory
    {
        // Keep the default timeout pretty short
        public static string ConnectionString
        {
            get
            {
                var currentDir = Directory.GetCurrentDirectory();
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile($"{currentDir}/appsettings.json")
                    .AddEnvironmentVariables("BEONTIME_")
                    .Build();

                return configuration.GetConnectionString("POSTGRESQL");
            }
        }

        static ConnectionSource()
        {
            if (ConnectionString.IsEmpty())
                throw new Exception(
                    "You need to set the connection string for your local Postgresql " +
                    "database in the environment variable 'BEONTIME_CONNECTIONSTRINGS__POSTGRESQL'");
        }


        public ConnectionSource() : base(ConnectionString)
        {
        }
    }
}

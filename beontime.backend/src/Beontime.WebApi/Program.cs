using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Beontime.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            
            try
            {
                Log.Information("Application Starting up!");
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start correctly!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var listenUrls = string.Empty;
            var portEnv = Environment.GetEnvironmentVariable("PORT") ?? "5048";
            if (!string.IsNullOrEmpty(portEnv) && !portEnv.Equals("80"))
            {
                listenUrls += $"http://*:{portEnv}";
            }
            
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(options =>
                {
                    options.AddEnvironmentVariables("BEONTIME_");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(listenUrls);
                    webBuilder.UseSerilog();
                });
        }
    }
}
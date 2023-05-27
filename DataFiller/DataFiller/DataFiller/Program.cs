using Autofac.Extensions.DependencyInjection;
using Common.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using WebFramework;

namespace DataFiller
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .AddJsonFile("appsettings.json")
                    .Build();

                CreateHostBuilder(args, configuration).Build().Run();
            }
            catch (Exception ex)
            {
                ErrorHandlerStartApp.WriteInFile(ex);
                Console.WriteLine(ex.ToJson());
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfigurationRoot configuration) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory()) //<-like yours
            .ConfigureAppConfiguration(builder =>
            {
                builder.Sources.Clear();
                builder.AddConfiguration(configuration);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseSerilog((hostingContext, loggerConfig) =>
                loggerConfig.ReadFrom
                .Configuration(hostingContext.Configuration));
            })
            .ConfigureWebHost(config =>
            {
                config.UseUrls("http://*:5053");
            }).UseWindowsService();
    }

}

using System.IO;
using BLL;
using Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PL.Console.Authorization;

namespace PL.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetService<App>()?.StartApp();
            //serviceProvider.GetService<Registration>();
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@"..\..\..\appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();
            
            //ToDo: add smtp host settings to appsettings.json
            services.Configure<AppSettings>(configuration.GetSection("AppSettings")); 
            
            services.AddScoped<App>();
            services.AddScoped<Registration>();
            DependencyRegistrar.ConfigureServices(services);
        }
    }
}

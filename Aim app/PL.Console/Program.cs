using BLL;
using Microsoft.Extensions.DependencyInjection;

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
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<App>();
            DependencyRegistrar.ConfigureServices(services);
        }
    }
}
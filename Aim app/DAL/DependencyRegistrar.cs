using DAL.Abstractions.Interfaces;
using DAL.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static class DependencyRegistrar
    {
        public static void ConfigureServices(IServiceCollection services) 
        {
            services.AddScoped<IWorker, JsonWorker>();
        }
    }
}
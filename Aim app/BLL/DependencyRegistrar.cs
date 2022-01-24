using BLL.Abstractions.Interfaces;
using BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class DependencyRegistrar
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            DAL.DependencyRegistrar.ConfigureServices(services);
        }
    }
}

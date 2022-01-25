using System.ComponentModel.DataAnnotations;
using System.Net;
using BLL.Abstractions.Interfaces;
using BLL.Helpers;
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
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IUserValidator, UserValidator>();
            services.AddScoped<IMailWorker, MailWorker>();
            DAL.DependencyRegistrar.ConfigureServices(services);
        }
    }
}

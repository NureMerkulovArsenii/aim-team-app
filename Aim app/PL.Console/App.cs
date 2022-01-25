using System;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using BLL.Helpers;
using PL.Console.Interfaces;
using PL.Console.Registration;

namespace PL.Console
{
    public class App
    {
        private readonly IUserService _userService;
        private readonly IRegistration _registration;
        private readonly Authorization.Authorization _authorization;

        public App(IUserService userService, IAuthorizationService authorizationService,IRegistration registration)
        {
            _userService = userService;
            _registration = registration;
            _authorization = new Authorization.Authorization(authorizationService);
        }

        public async Task StartApp()
        {
            var userKeys = new[] {"y", "n"};
            string key;
            do
            {
                System.Console.WriteLine("Wanna sign up (press \"y\"), wanna sign in (press \"n\")");
                key = System.Console.ReadLine();
            } while (key?.Length != 1 && !userKeys.Contains(key));

            if (key == "y")
            {
                await _registration.RegisterUserAsync();
            }
            else if (key == "n")
            {
                var response = await _authorization.AuthorizeUserAsync();

                if (response)
                {
                    System.Console.WriteLine("Successfully logged in!");
                }
                else
                {
                    System.Console.WriteLine("Login failed, please try again later!");
                }
            }
        }
    }
}

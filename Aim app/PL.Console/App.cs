using System;
using System.Linq;
using BLL.Abstractions.Interfaces;
using BLL.Services;
using PL.Console.Authorization;


namespace PL.Console
{
    public class App
    {
        private readonly IUserService _userService;
        private readonly Registration _registration;

        public App(IUserService userService, IRegistrationService registrationService)
        {
            _userService = userService;
            _registration = new Registration(registrationService);
        }

        public void StartApp()
        {
            var userKeys = new[] {"y", "n"};
            string key;
            do
            {
                System.Console.WriteLine("Wanna sign up (press \"y\"), wanna sign in (press) \"n\"");
                key = System.Console.ReadLine();
            } while (key.Length != 1 && !userKeys.Contains(key));

            if (key == "y")
            {
                _registration.RegisterUserAsync();
            }
        }
    }
}

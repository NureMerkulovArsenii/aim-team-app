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
        private readonly IAuthorization _authorization;
        private readonly IRoomsControl _roomsControl;

        public App(IUserService userService, IAuthorization authorization, IRegistration registration, IRoomsControl roomsControl)
        {
            _userService = userService;
            _registration = registration;
            _authorization = authorization;
            _roomsControl = roomsControl;
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

            while (true)
            {
                await _roomsControl.ShowUserRooms();
            }
        }
    }
}

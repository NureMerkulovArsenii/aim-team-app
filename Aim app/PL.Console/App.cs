using System;
using BLL.Abstractions.Interfaces;

namespace PL.Console
{
    public class App
    {
        private readonly IUserService _userService;
        
        public App(IUserService userService)
        {
            _userService = userService;
        }
        
        public void StartApp()
        {
            System.Console.WriteLine("Started!");
        }
    }
}
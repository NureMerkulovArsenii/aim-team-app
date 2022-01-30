using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using BLL.Helpers;
using Core;
using Microsoft.Extensions.Options;
using PL.Console.Interfaces;

namespace PL.Console.Registration
{
    public class Registration : IRegistration
    {
        private readonly IRegistrationService _registrationService;
        private readonly IPasswordService _passwordService;
        private readonly IUserValidator _validator;
        private readonly IMailWorker _mailWorker;
        private readonly ICurrentUser _currentUser;

        public Registration(IRegistrationService registrationService, IPasswordService passwordService,
            IUserValidator validator, IMailWorker mailWorker, ICurrentUser currentUser)
        {
            this._passwordService = passwordService;
            this._registrationService = registrationService;
            this._validator = validator;
            this._mailWorker = mailWorker;
            this._currentUser = currentUser;
        }

        public async Task RegisterUserAsync()
        {
            System.Console.WriteLine("Enter your Name: ");
            var name = System.Console.ReadLine();
            System.Console.WriteLine("Enter your Surname: ");
            var surName = System.Console.ReadLine();
            System.Console.WriteLine("Enter your Nickname: ");
            var nickName = System.Console.ReadLine();

            while (!_validator.ValidateUserNick(nickName))
            {
                System.Console.WriteLine("Nickname you have chosen is busy\n try another one:");
                nickName = System.Console.ReadLine();
            }

            System.Console.WriteLine("Enter your email");
            var email = System.Console.ReadLine();

            // Notice: if Validator.cs is no longer needed, change to if (new EmailAddressAttribute().IsValid("someone@somewhere.com")) 
            var emailValidationResult = _validator.IsEmailValid(email);
            while (emailValidationResult != 0)
            {
                if (emailValidationResult == 1)
                {
                    System.Console.WriteLine("User with this email already exists");
                    email = System.Console.ReadLine();
                    emailValidationResult = _validator.IsEmailValid(email);
                }

                if (emailValidationResult == -1)
                {
                    System.Console.WriteLine("Incorrect email format, try again:");
                    email = System.Console.ReadLine();
                    emailValidationResult = _validator.IsEmailValid(email);
                }
            }

            System.Console.WriteLine("Enter your password");
            var password = System.Console.ReadLine();

            while (!_passwordService.HasPasswordCorrectFormat(email, password))
            {
                System.Console.WriteLine("Incorrect password, try again:");
                password = System.Console.ReadLine();
            }


            System.Console.WriteLine("Check your email and enter code:");

            var code = System.Console.ReadLine();

            while (_mailWorker.CompareCodes(code))
            {
                System.Console.WriteLine("Wrong code");
                System.Console.WriteLine(
                    "Enter \"y\" if you want to reenter code from email and \"n\" if you want to send it again");
                var userAnswer = System.Console.ReadLine();
                if (userAnswer == "n")
                {
                    await _mailWorker.SendCodeByEmailAsync(email);
                    System.Console.WriteLine("Check your email and enter code:");
                    code = System.Console.ReadLine();
                }

                if (userAnswer == "y")
                {
                    code = System.Console.ReadLine();
                }
            }

            await _registrationService.RegisterAsync(email, name, surName, nickName, password, true);
            System.Console.WriteLine("You have just registered successfully");
            
            var tempUser = new User()
            {
                UserName = nickName,
                Email = email,
                FirstName = name,
                LastName = surName,
                IsVerified = true,
                LastAuth = DateTime.Now
            };
            _passwordService.SetPassword(tempUser, password);
            _currentUser.User = tempUser;
        }
    }
}

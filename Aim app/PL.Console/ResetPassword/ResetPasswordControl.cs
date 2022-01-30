﻿using System;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using PL.Console.Interfaces;

namespace PL.Console.ResetPassword
{
    public class ResetPasswordControl : IResetPasswordControl
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMailWorker _mailWorker;
        private readonly IPasswordService _passwordService;

        public ResetPasswordControl(IUserService userService, IAuthorizationService authorizationService,
            IMailWorker mailWorker, IPasswordService passwordService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
            _mailWorker = mailWorker;
            _passwordService = passwordService;
        }

        public async Task<bool> ResetUserPasswordAsync()
        {
            try
            {
                System.Console.Write("Enter your username/email: ");
                var usernameOrEmail = System.Console.ReadLine()?.Trim();

                var userToReset = await _userService.GetUserByUserNameOrEmail(usernameOrEmail);

                while (userToReset == null)
                {
                    System.Console.WriteLine("Invalid username or email");

                    System.Console.Write("Enter your username/email: ");
                    usernameOrEmail = System.Console.ReadLine()?.Trim();

                    userToReset = await _userService.GetUserByUserNameOrEmail(usernameOrEmail);
                }

                var email = await _authorizationService.GetEmailByUsernameOrEmail(usernameOrEmail);
                await _mailWorker.SendCodeByEmailAsync(email);

                System.Console.WriteLine($"We have sent message on your email ({email}) with code");
                System.Console.Write("Enter code to reset your password: ");
                var codeFromUser = System.Console.ReadLine()?.Trim();

                while (_mailWorker.CompareCodes(codeFromUser))
                {
                    System.Console.Write(
                        "Wrong code! Enter code from message sent on your email or \"r\" to resend code: ");
                    codeFromUser = System.Console.ReadLine()?.Trim();

                    if (codeFromUser == "r")
                    {
                        await _mailWorker.SendCodeByEmailAsync(email);
                    }
                }

                string password;
                do
                {
                    System.Console.WriteLine("Enter new password: ");
                    password = System.Console.ReadLine()?.Trim();
                } while (string.IsNullOrWhiteSpace(password) ||
                         !_passwordService.HasPasswordCorrectFormat(email, password));

                return await _passwordService.SetPassword(userToReset, password, true);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
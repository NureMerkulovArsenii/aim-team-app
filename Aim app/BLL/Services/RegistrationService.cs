using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;
using Microsoft.Extensions.Options;

namespace BLL.Services
{
    public class RegistrationService : IRegistrationService
    {
        public string Code { get; set; }

        private readonly IPasswordService _passwordService;
        private readonly IGenericRepository<User> _genericRepository;

        public RegistrationService(IPasswordService passwordService, IGenericRepository<User> genericRepository)
        {
            this._passwordService = passwordService;
            this._genericRepository = genericRepository;
        }

        public async Task RegisterAsync(string userMail, string name, string surname, string nickName, string password,
            bool isVerified = false)
        {
            var user = new User()
            {
                Email = userMail,
                FirstName = name,
                LastName = surname,
                UserName = nickName,
                IsVerified = isVerified,
                LastAuth = DateTime.Now
            };

            _passwordService.SetPassword(user, password);


            await _genericRepository.CreateAsync(user);
        }
    }
}

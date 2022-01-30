using System;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class RegistrationService : IRegistrationService
    {
        public string Code { get; set; }

        private readonly IPasswordService _passwordService;
        private readonly IGenericRepository<User> _genericRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IAuthorizationService _authorizationService;

        public RegistrationService(IPasswordService passwordService, IGenericRepository<User> genericRepository,
            ICurrentUser currentUser, IAuthorizationService authorizationService)
        {
            this._passwordService = passwordService;
            this._genericRepository = genericRepository;
            this._currentUser = currentUser;
            this._authorizationService = authorizationService;
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

            await _passwordService.SetPassword(user, password);
            await _genericRepository.CreateAsync(user);
            
            _currentUser.User = user;
            await _authorizationService.UpdateLastAuth(user);
        }
    }
}

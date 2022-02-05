using System;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IGenericRepository<User> _userGenericRepository;
        private readonly IUserValidator _userValidator;
        private readonly IPasswordService _passwordService;

        public AuthorizationService(IGenericRepository<User> userGenericRepository, IUserValidator userValidator,
            IPasswordService passwordService)
        {
            _userGenericRepository = userGenericRepository;
            _userValidator = userValidator;
            _passwordService = passwordService;
        }

        public async Task<bool> CheckUserDataForAuthAsync(string usernameOrEmail, string password)
        {
            if (_passwordService.HasPasswordCorrectFormat(usernameOrEmail, password))
            {
                var tempUser = new User();
                await _passwordService.SetPassword(tempUser, password);
                var passwordHashed = tempUser.Password;
                var receivedUsers = await _userGenericRepository.FindByConditionAsync(user =>
                    user.Password == passwordHashed &&
                    (user.Email == usernameOrEmail || user.UserName == usernameOrEmail));

                return receivedUsers.Any();
            }

            return false;
        }

        public async Task<string> GetEmailByUsernameOrEmail(string usernameOrEmail)
        {
            var response = await _userValidator.IsEmailValid(usernameOrEmail);
            if (response == 1)
            {
                return usernameOrEmail;
            }

            var userWithEmail =
                await _userGenericRepository.FindByConditionAsync(user => user.UserName == usernameOrEmail);
            
            return userWithEmail.FirstOrDefault()?.Email;
        }

        public async Task<bool> IsLastAuthWasLongAgo(User user, int numberOfDays)
        {
            var userLastAuth = user.LastAuth;

            return userLastAuth.AddDays(numberOfDays) < DateTime.Now;
        }

        public async Task UpdateLastAuth(User user)
        {
            user.LastAuth = DateTime.Now;

            await _userGenericRepository.UpdateAsync(user);
        }

        public async Task<User> GetInfoAboutUser(string usernameOrEmail)
        {
            var usersFromDb = await _userGenericRepository.FindByConditionAsync(user =>
                user.Email == usernameOrEmail || user.UserName == usernameOrEmail);
            
            return usersFromDb.FirstOrDefault();
        }
    }
}

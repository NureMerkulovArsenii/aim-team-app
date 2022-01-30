using System;
using System.Net.Mail;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using DAL.Repository;

namespace BLL.Helpers
{
    public class UserValidator : IUserValidator
    {
        private readonly IGenericRepository<User> _genericRepository;

        public UserValidator(IGenericRepository<User> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<int> IsEmailValid(string email)
        {
            try
            {
                var users = await _genericRepository.FindByConditionAsync(user => user.Email == email);
                if (users.Any())
                {
                    return 1;
                }

                return 0;
            }
            catch (FormatException)
            {
                return -1;
            }
        }


        public async Task<bool> ValidateUserNick(string nick)
        {
            var result = await _genericRepository.FindByConditionAsync(user => user.UserName == nick);
            if (result.Any())
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ValidateUserNameOrEmail(string userName)
        {
            var result = await _genericRepository
                .FindByConditionAsync(user => user.UserName == userName || user.Email == userName);
            
            return result.Any();
        }
    }
}

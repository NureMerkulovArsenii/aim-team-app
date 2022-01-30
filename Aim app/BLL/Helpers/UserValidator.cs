using System;
using System.Net.Mail;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;
using System.Linq;

namespace BLL.Helpers
{
    public class UserValidator : IUserValidator
    {
        private readonly IGenericRepository<User> _genericRepository;

        public UserValidator(IGenericRepository<User> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public int IsEmailValid(string email)
        {
            try
            {
                var address = new MailAddress(email).Address;
                var users = _genericRepository.FindByConditionAsync(user => user.Email == email).Result;
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


        public bool ValidateUserNick(string nick)
        {
            var result = _genericRepository.FindByConditionAsync(user => user.UserName == nick).Result;
            if (result.Any())
            {
                return false;
            }

            return true;
        }

        public bool ValidateUserNameOrEmail(string userName)
        {
            var result = _genericRepository
                .FindByConditionAsync(user => user.UserName == userName || user.Email == userName).Result;
            if (result.Any())
            {
                return true;
            }

            return false;
        }
    }
}

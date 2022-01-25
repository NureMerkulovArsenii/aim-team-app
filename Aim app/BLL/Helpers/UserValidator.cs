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

        public bool IsEmailValid(string email)
        {
            try
            {
                var address = new MailAddress(email).Address;
                return true;
            }
            catch (FormatException)
            {
                return false;
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
    }
}

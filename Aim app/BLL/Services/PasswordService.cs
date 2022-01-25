using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using BLL.Abstractions.Interfaces;
using Core;

namespace BLL.Services
{
    public class PasswordService : IPasswordService
    {
        public bool ChangePassword(User user, string oldPassword, string newPassword)
        {
            return IsPasswordCorrect(user, oldPassword) ? SetPassword(user, newPassword) : false;
        }

        public bool SetPassword(User user, string password)
        {
            if (HasPasswordCorrectFormat(user, password))
            {
                user.Password = GetHash(password);
                //TODO: Save User

                return true;
            }

            return false;
        }

        public bool HasPasswordCorrectFormat(User user, string password)
        {
            if (!string.IsNullOrWhiteSpace(password) && password.Length >= 8 && password.Length <= 24)
            {
                Regex regex = new("([A-Za-z0-9!\"#%&'()*,\\-./:;?@[\\\\\\]_{}¡«­·»¿;])*");
                var searchedString = regex.Match(password).Value;

                if (searchedString == password && user.Email != password)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsPasswordCorrect(User user, string password)
        {
            var passHash = GetHash(password);

            return passHash == user.Password;
        }

        private string GetHash(string password)
        {
            var passBytes = Encoding.UTF8.GetBytes(password);
            var passHash = SHA256.Create().ComputeHash(passBytes);

            return Encoding.UTF8.GetString(passHash);
        }
    }
}

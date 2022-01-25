using System;
using System.Net.Mail;
using BLL.Abstractions.Interfaces;

namespace BLL.Helpers
{
    public class Validator : IValidator
    {
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
    }
}

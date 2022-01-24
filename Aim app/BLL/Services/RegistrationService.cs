using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;

namespace BLL.Services
{
    public class RegistrationService : IRegistrationService
    { public string Code { get; set; }
        private readonly SmtpClient _smtpClient;
        private readonly AppSettings _appSettings;
        
        

        public RegistrationService()
        {
            _appSettings = new AppSettings();
            
            _smtpClient = new SmtpClient
            {
                Host = _appSettings.SmtpSettings.Host,
                Port = _appSettings.SmtpSettings.Port,
                EnableSsl = _appSettings.SmtpSettings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = _appSettings.SmtpSettings.UseDefaultCredentials
            };

        }

        public async Task RegisterAsync(string emailTo, string passwordHash)
        {
            try
            {
                var random = new Random();
                Code = random.Next(100000, 999999).ToString(CultureInfo.InvariantCulture);

                var fromAddress = new MailAddress(_appSettings.Email, "AIM-team");
                var toAddress = new MailAddress(emailTo);
                var password = "xjzloridcwckfutg";

                _smtpClient.Credentials = new NetworkCredential(fromAddress.Address, password);


                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = "Verify account",
                    Body = Code
                };

                await _smtpClient.SendMailAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public string GetHashFromString(string input)
        {
            return input;
        }
        
        public bool CompareCodes(string userCode)
        {
            if (Code == userCode)
            {
                return true;
            }

            return false;
        }
    }
}

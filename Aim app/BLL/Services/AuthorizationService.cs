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
    public class AuthorizationService : IAuthorizationService
    {
        public string Code { get; set; }
        private readonly AppSettings _appSettings;
        private readonly IGenericRepository<User> _genericRepository;

        public AuthorizationService(IGenericRepository<User> genericRepository, IOptions<AppSettings> appSettings)
        {
            _genericRepository = genericRepository;
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public async Task<bool> CheckUserDataForLoginAsync(string loginOrEmail, string password)
        {
            // Запрос к БД и поиск такого юзера, если юзер найден:
            return true;
        }

        public async Task<string> SendCodeByEmailAsync(string emailTo)
        {
            var random = new Random();
            Code = random.Next(100000, 999999).ToString(CultureInfo.InvariantCulture);
            var toMailAddress = new MailAddress(emailTo);
            var subject = "AIM APP | Confirmation code";
            var body =
                $"<h4>Confirmation code to enter the application:</h4><br><center><code><b>{Code}</b></center></code>";
        
            var resultOfSending = await SendMailMessageAsync(toMailAddress, subject, body);

            return resultOfSending ? Code : "0";
        }

        public async Task ResendCodeByEmailAsync(string emailTo)
        {
            var toMailAddress = new MailAddress(emailTo);
            var subject = "AIM APP | Confirmation code";
            var body =
                $"<h4>Confirmation code to enter the application:</h4><br><center><code><b>{Code}</b></center></code>";
        
            await SendMailMessageAsync(toMailAddress, subject, body);
        }

        public bool CompareCodes(string codeFromUser)
        {
            return codeFromUser == Code;
        }

        private async Task<bool> SendMailMessageAsync(MailAddress mailAddressTo, string subject, string body)
        {
            try
            {
                var fromMailAddress = new MailAddress(_appSettings.Email, _appSettings.EmailDisplayName);

                var message = new MailMessage(fromMailAddress, mailAddressTo);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
            
                var smtpClient = new SmtpClient(_appSettings.SmtpSettings.Host, _appSettings.SmtpSettings.Port);
                smtpClient.Credentials = new NetworkCredential(fromMailAddress.Address, _appSettings.SmtpSettings.Password);
                smtpClient.EnableSsl = true;
            
                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

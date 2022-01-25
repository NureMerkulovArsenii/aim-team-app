using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using Microsoft.Extensions.Options;

namespace BLL.Helpers
{
    public class MailWorker : IMailWorker
    {
        public string Code { get; set; }
        private readonly AppSettings _appSettings;

        public MailWorker(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
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

        public async Task<string> SendCodeByEmailAsync(string emailTo, string code)
        {
            var toMailAddress = new MailAddress(emailTo);
            var subject = "AIM APP | Confirmation code";
            var body =
                $"<h4>Confirmation code to enter the application:</h4><br><center><code><b>{code}</b></center></code>";

            var resultOfSending = await SendMailMessageAsync(toMailAddress, subject, body);

            return resultOfSending ? code : "0";
        }
        
        public async Task<bool> SendMailMessageAsync(MailAddress mailAddressTo, string subject, string body)
        {
            try
            {
                var fromMailAddress = new MailAddress(_appSettings.Email, _appSettings.EmailDisplayName);

                var message = new MailMessage(fromMailAddress, mailAddressTo);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient(_appSettings.SmtpSettings.Host, _appSettings.SmtpSettings.Port);
                smtpClient.Credentials =
                    new NetworkCredential(fromMailAddress.Address, _appSettings.SmtpSettings.Password);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

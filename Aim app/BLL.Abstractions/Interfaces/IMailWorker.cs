using System.Net.Mail;
using System.Threading.Tasks;

namespace BLL.Abstractions.Interfaces
{
    public interface IMailWorker
    {
        string Code { get; set; }

        Task<string> SendCodeByEmailAsync(string emailTo);

        Task<bool> SendMailMessageAsync(MailAddress mailAddressTo, string subject, string body);
    }
}

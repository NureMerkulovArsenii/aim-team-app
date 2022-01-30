using System.Net.Mail;
using System.Threading.Tasks;

namespace BLL.Abstractions.Interfaces
{
    public interface IMailWorker
    {
        string Code { get; set; }

        Task<bool> SendCodeByEmailAsync(string emailTo);

        bool CompareCodes(string codeFromUser);

        // Task<string> SendCodeByEmailAsync(string emailTo, string code);
    }
}

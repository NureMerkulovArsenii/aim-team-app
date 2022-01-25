using System.Threading.Tasks;

namespace BLL.Abstractions.Interfaces
{
    public interface IAuthorizationService
    {
        string Code { get; set; }
        Task<bool> CheckUserDataForLoginAsync(string loginOrEmail, string password);
        Task<string> SendCodeByEmailAsync(string emailTo);
        Task ResendCodeByEmailAsync(string emailTo);
        bool CompareCodes(string codeFromUser);
    }
}

using System.Threading.Tasks;

namespace BLL.Abstractions.Interfaces
{
    public interface IRegistrationService
    {
        string Code { get; set; }
        Task RegisterAsync(string emailTo, string passwordHash);
        string GetHashFromString(string input);
        bool CompareCodes(string userCode);
    }
}

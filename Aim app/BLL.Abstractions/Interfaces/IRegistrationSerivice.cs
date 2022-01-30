using System.Threading.Tasks;

namespace BLL.Abstractions.Interfaces
{
    public interface IRegistrationService
    {
        string Code { get; set; }
        
        public Task RegisterAsync(string emailTo, string name, string surname, string nickName, string password, bool isVerified);
    }
}

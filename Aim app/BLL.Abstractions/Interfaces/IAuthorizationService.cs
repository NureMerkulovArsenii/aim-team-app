using System.Threading.Tasks;

namespace BLL.Abstractions.Interfaces
{
    public interface IAuthorizationService
    {
        Task<bool> CheckUserDataForAuthAsync(string usernameOrEmail, string password);
        
        Task<string> GetEmailByUsernameOrEmail(string usernameOrEmail);

        Task<bool> IsLastAuthWasLongAgo(string usernameOrEmail, int numberOfDays);
        
        Task UpdateLastAuth(string usernameOrEmail);
    }
}

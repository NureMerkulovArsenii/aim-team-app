using System.Threading.Tasks;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserValidator
    {
        // Todo: Make async
        Task<int> IsEmailValid(string email);
        Task<bool> ValidateUserNick(string nick);
        Task<bool> ValidateUserNameOrEmail(string userName);
    }
}

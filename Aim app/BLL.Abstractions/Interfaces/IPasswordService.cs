using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IPasswordService
    {
        bool ChangePassword(string oldPassword, string newPassword);

        bool SetPassword(User user, string password);

        bool HasPasswordCorrectFormat(string email, string password);

        bool IsPasswordCorrect(User user, string password);
    }
}

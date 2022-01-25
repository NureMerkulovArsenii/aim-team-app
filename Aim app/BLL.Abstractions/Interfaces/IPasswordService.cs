using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IPasswordService
    {
        bool ChangePassword(User user, string oldPassword, string newPassword);

        bool SetPassword(User user, string password);

        bool HasPasswordCorrectFormat(string email, string password);

        bool IsPasswordCorrect(User user, string password);
    }
}

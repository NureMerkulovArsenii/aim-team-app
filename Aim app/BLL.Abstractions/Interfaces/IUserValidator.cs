namespace BLL.Abstractions.Interfaces
{
    public interface IUserValidator
    {
        // Todo: Make async
        int IsEmailValid(string email);
        public bool ValidateUserNick(string nick);
        bool ValidateUserNameOrEmail(string userName);
    }
}

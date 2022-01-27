namespace BLL.Abstractions.Interfaces
{
    public interface IUserValidator
    {
        int IsEmailValid(string email);
        public bool ValidateUserNick(string nick);
        bool ValidateUserNameOrEmail(string userName);
    }
}

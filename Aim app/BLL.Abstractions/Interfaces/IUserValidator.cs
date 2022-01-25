namespace BLL.Abstractions.Interfaces
{
    public interface IUserValidator
    {
        bool IsEmailValid(string email);
    }
}

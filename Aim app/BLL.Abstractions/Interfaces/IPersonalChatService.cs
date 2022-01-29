using System.Threading.Tasks;

namespace BLL.Abstractions.Interfaces
{
    public interface IPersonalChatService
    {
        Task CreatePersonalChat(string userToChatWith);
    }
}

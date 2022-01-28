using System.Threading.Tasks;
using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface ITextChatService
    {
        Task<bool> CreateTextChat(Room room, string name, string description, bool isAdmin);
    }
}

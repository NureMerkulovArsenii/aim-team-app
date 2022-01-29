using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IPersonalChatService
    {
        Task CreatePersonalChat(string[] userToChatWith);
        Task<bool> ChangeNameOfPersonalChat(string chatName, string name);
        Task AddParticipantsToPersonalChat(string chatName, string[] participants);
        Task<IList<string>> GetUserPersonalChats();
    }
}

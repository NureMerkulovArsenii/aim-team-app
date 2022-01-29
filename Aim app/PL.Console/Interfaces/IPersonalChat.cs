using System.Collections.Generic;
using System.Threading.Tasks;

namespace PL.Console.Interfaces
{
    public interface IPersonalChat
    {
        void StartChat();
        Task GetUserPersonalChats();
    }
}

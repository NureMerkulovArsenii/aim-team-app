using System.Threading.Tasks;
using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserService
    {
        Task LeaveRoom(string roomId);
        Task SwitchNotifications(string roomId, bool stateOnOrOff);
    }
}

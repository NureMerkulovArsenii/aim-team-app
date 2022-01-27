using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IUserService
    {
        Task<bool> LeaveRoom(string roomId);
        Task<bool> SwitchNotifications(string roomId, bool stateOnOrOff);
        bool IsUserVerified(User user);
        Task<List<Room>> GetUserRooms();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IRoomService
    {
        //ToDo: Make Async
        int CreateRoom(string name, string description);

        bool DeleteRoom(Room room);

        bool ChangeRoomSettings(Room room, string name, string description);
        
        Task<List<User>> GetParticipantsOfRoom(int roomId);
    }
}

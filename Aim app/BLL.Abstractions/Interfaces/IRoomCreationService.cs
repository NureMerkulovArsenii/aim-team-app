using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IRoomCreationService
    {
        string CreateRoom(string name, string description);

        bool DeleteRoom(Room room);

        bool ChangeRoomSettings(Room room, string name, string description);
    }
}

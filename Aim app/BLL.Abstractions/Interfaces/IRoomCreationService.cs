using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IRoomCreationService
    {
        string CreateRoom(User user, string name, string description);

        bool DeleteRoom(Room room, User user);

        bool ChangeRoomSettings(Room room, User user, string name, string description);
    }
}

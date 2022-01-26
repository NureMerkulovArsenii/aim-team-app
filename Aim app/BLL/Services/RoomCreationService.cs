using System.Collections.Generic;
using System.Linq;
using Core;
using DAL.Abstractions.Interfaces;
using BLL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class RoomCreationService : IRoomCreationService
    {
        private readonly IGenericRepository<Room> _roomGenericRepository;

        public RoomCreationService(IGenericRepository<Room> roomRepository)
        {
            _roomGenericRepository = roomRepository;
        }

        public string CreateRoom(User user, string name, string description) //TODO: Photo implementation
        {
            var baseParticipantInfo = new ParticipantInfo()
            {
                RoleId = "1",
                Notifications = true
            };

            var room = new Room()
            {
                RoomName = name,
                RoomDescription = description,
                Participants = new Dictionary<User, ParticipantInfo> { { user, baseParticipantInfo } }
            };

            _roomGenericRepository.CreateAsync(room).Wait();

            return room.Id;
        }

        public bool DeleteRoom(Room room, User user)
        {
            if (IsUserAdmin(room, user))
            {
                _roomGenericRepository.DeleteAsync(room).Wait();

                return true;
            }

            return false;
        }

        public bool ChangeRoomSettings(Room room, User user, string name, string description) //TODO: photo implementation
        {
            if (IsUserAdmin(room, user))
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    room.RoomName = name;
                }
                if (!string.IsNullOrWhiteSpace(description))
                {
                    room.RoomDescription = description;
                }

                _roomGenericRepository.UpdateAsync(room).Wait();

                return true;
            }

            return false;
        }

        private bool IsUserAdmin(Room room, User user) //TODO: roles implementation
        {
            var isUserAdmin = room.Participants.Where(userPair => userPair.Value.RoleId == "1" && userPair.Key.Id == user.Id);

            return isUserAdmin.Any();
        }
    }
}

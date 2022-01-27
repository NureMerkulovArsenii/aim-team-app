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
        private readonly ICurrentUser _currentUser;

        public RoomCreationService(IGenericRepository<Room> roomRepository, ICurrentUser currentUser)
        {
            _roomGenericRepository = roomRepository;
            _currentUser = currentUser;
        }

        public string CreateRoom(string name, string description) //TODO: Photo implementation
        {
            var user = _currentUser.User;
            var baseParticipantInfo = new ParticipantInfo()
            {
                User = user,
                RoleId = "1",
                Notifications = true
            };

            var room = new Room()
            {
                RoomName = name,
                RoomDescription = description,
                Participants = new List<ParticipantInfo> { baseParticipantInfo }
            };

            _roomGenericRepository.CreateAsync(room).Wait();

            return room.Id;
        }

        public bool DeleteRoom(Room room)
        {
            var user = _currentUser.User;
            
            if (IsUserAdmin(room, user))
            {
                _roomGenericRepository.DeleteAsync(room).Wait();

                return true;
            }

            return false;
        }

        public bool ChangeRoomSettings(Room room, string name, string description) //TODO: photo implementation
        {
            var user = _currentUser.User;
            
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
            var isUserAdmin = room.Participants.Where(participant => participant.RoleId == "1" && participant.User.Id == user.Id);

            return isUserAdmin.Any();
        }
    }
}

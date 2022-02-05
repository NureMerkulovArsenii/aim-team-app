using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class RoomService : IRoomService
    {
        private readonly IGenericRepository<Room> _roomGenericRepository;
        private readonly IGenericRepository<User> _userGenericRepository;
        private readonly IGenericRepository<Role> _roleGenericRepository;
        private readonly ICurrentUser _currentUser;

        public RoomService(IGenericRepository<Room> roomGenericRepository, ICurrentUser currentUser,
            IGenericRepository<User> userGenericRepository, IGenericRepository<Role> roleGenericRepository)
        {
            _roomGenericRepository = roomGenericRepository;

            _currentUser = currentUser;

            _userGenericRepository = userGenericRepository;

            _roleGenericRepository = roleGenericRepository;
        }

        public int CreateRoom(string name, string description) //TODO: Photo implementation
        {
            var user = _currentUser.User;

            var adminRole = new Role("Admin")
            {
                CanInvite = true,
                CanDeleteOthersMessages = true,
                CanManageChannels = true,
                CanManageRoles = true,
                CanManageRoom = true,
                CanModerateParticipants = true,
                CanPin = true,
                CanUseAdminChannels = true,
                CanViewAuditLog = true
            };

            var baseRole = new Role("User");

            var adminParticipantInfo = new ParticipantInfo()
            {
                Notifications = true, User = user, Role = adminRole
            };

            var room = new Room()
            {
                RoomName = name,
                RoomDescription = description,
                Participants = new List<ParticipantInfo> {adminParticipantInfo},
                RoomRoles = new List<Role>() {adminRole, baseRole},
                TextChannels = new List<TextChannel>()
            };

            _roleGenericRepository.CreateAsync(adminRole).Wait();

            _roleGenericRepository.CreateAsync(baseRole).Wait();

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

        private bool IsUserAdmin(Room room, User user)
        {
            var isUserAdmin = room.Participants
                .Where(participant => participant.Role.CanManageRoom && participant.User.Id == user.Id);

            return isUserAdmin.Any();
        }

        public async Task<List<User>> GetParticipantsOfRoom(int roomId)
        {
            var room = _roomGenericRepository
                .FindByConditionAsync(room => room.Id == roomId)
                .Result
                .FirstOrDefault();

            var users = room?.Participants.Select(participant => participant.User).ToList();

            return users;
        }
    }
}

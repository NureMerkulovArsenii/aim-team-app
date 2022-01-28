using System;
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

        public string CreateRoom(string name, string description) //TODO: Photo implementation
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
                UserId = user.Id, RoleId = adminRole.Id, Notifications = true
            };

            var room = new Room()
            {
                RoomName = name,
                RoomDescription = description,
                BaseRoleId = baseRole.Id,
                Participants = new List<ParticipantInfo> {adminParticipantInfo},
                RoomRolesId = new List<string>() {adminRole.Id, baseRole.Id}
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
                .Where(participant => _roleGenericRepository
                    .GetEntityById(participant.RoleId).Result.CanManageRoom && participant.UserId == user.Id);

            return isUserAdmin.Any();
        }

        public async Task<List<User>> GetParticipantsOfRoom(string roomId)
        {
            var room = _roomGenericRepository
                .FindByConditionAsync(room => room.Id == roomId)
                .Result
                .FirstOrDefault();

            var userIds = room?.Participants.Select(participant => participant.UserId).ToList();

            return (List<User>)await _userGenericRepository
                .FindByConditionAsync(user => userIds.Contains(user.Id));
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class RoleService : IRoleService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IGenericRepository<Role> _roleGenericRepository;
        private readonly IGenericRepository<Room> _roomGenericRepository;
        private readonly IGenericRepository<User> _userGenericRepository;

        public RoleService(ICurrentUser currentUser, IGenericRepository<Role> roleGenericRepository,
            IGenericRepository<Room> roomGenericRepository, IGenericRepository<User> userGenericRepository)
        {
            _currentUser = currentUser;
            _roleGenericRepository = roleGenericRepository;
            _roomGenericRepository = roomGenericRepository;
            _userGenericRepository = userGenericRepository;
        }

        public async Task<bool> SetUpRole(Room room, Role role, IDictionary<string, bool?> permissions)
        {
            var user = _currentUser.User;
            var participant = room.Participants.FirstOrDefault(participant => participant.User.Id == user.Id);
            var userRole = participant?.Role;

            if (!userRole.CanManageRoles || room.RoomRoles.All(roomRole => roomRole.Id != role.Id))
            {
                return false;
            }

            foreach (var pair in permissions)
            {
                if (pair.Value is { } value)
                {
                    role.Permissions[pair.Key] = value;
                }
            }

            await _roleGenericRepository.UpdateAsync(role);

            return true;
        }

        public async Task<Role> CreateNewRole(Room room, string name)
        {
            if (!CanManageRoles(room))
            {
                return null;
            }

            var newRole = new Role(name);
            
            room.RoomRoles.Add(newRole);

            await _roleGenericRepository.CreateAsync(newRole);

            await _roomGenericRepository.UpdateAsync(room);

            return newRole;
        }

        public async Task<bool> DeleteRole(Room room, Role role)
        {
            if (!CanManageRoles(room) || room.BaseRole.Id == role.Id ||
                room.RoomRoles.Any(roomRole => roomRole.Id == role.Id))
            {
                return false;
            }
            
            room.RoomRoles.Remove(role);

            await _roomGenericRepository.UpdateAsync(room);

            await _roleGenericRepository.DeleteAsync(role);

            return true;
        }

        public async Task<List<Role>> GetAllRolesInRoom(Room room)
        {
            var resultRoles = new List<Role>();

            foreach (var role in room.RoomRoles)
            {
                resultRoles.Add(role);
            }

            return resultRoles;
        }

        public async Task<bool> SetRoleToUser(Room room, User user, Role role)
        {
            if (!CanManageRoles(room) || room.RoomRoles.Any(roomRole => roomRole.Id == role.Id))
            {
                return false;
            }

            var participant = room.Participants.FirstOrDefault(participant => participant.User.Id == user.Id);

            if (participant == null)
            {
                return false;
            }
            
            participant.Role = role;

            await _roomGenericRepository.UpdateAsync(room);
            
            return true;
        }

        public async Task<Dictionary<string, string>> GetRolesOfUsers(int roomId)
        {
            var result = new Dictionary<string, string>();

            var room = _roomGenericRepository
                .FindByConditionAsync(room => room.Id == roomId)
                .Result
                .FirstOrDefault();

            var roomParticipants = room?.Participants;

            foreach (var participant in roomParticipants)
            {
                var user = participant.User;
                var role = participant.Role;

                result.Add(user.UserName, role.RoleName);
            }

            return result;
        }

        private bool CanManageRoles(Room room)
        {
            var user = _currentUser.User;
            var participant = room.Participants.FirstOrDefault(participant => participant.User.Id == user.Id);
            var userRole = participant?.Role;

            if (userRole.CanManageRoles)
            {
                return true;
            }

            return false;
        }
    }
}

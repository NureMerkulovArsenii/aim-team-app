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
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(ICurrentUser currentUser, IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
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
            
            await _unitOfWork.RoleRepository.UpdateAsync(role);

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

            try
            {
                await _unitOfWork.CreateTransactionAsync();

                await _unitOfWork.RoleRepository.CreateAsync(newRole);

                await _unitOfWork.RoomRepository.UpdateAsync(room);

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
            }

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

            try
            {
                await _unitOfWork.CreateTransactionAsync();

                await _unitOfWork.RoleRepository.DeleteAsync(role);
                
                await _unitOfWork.RoomRepository.UpdateAsync(room);

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
            }

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
            
            await _unitOfWork.RoomRepository.UpdateAsync(room);
            
            return true;
        }

        public async Task<Dictionary<string, string>> GetRolesOfUsers(int roomId)
        {
            var result = new Dictionary<string, string>();

            var room = _unitOfWork.RoomRepository
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

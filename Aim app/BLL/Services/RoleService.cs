using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        public RoleService(ICurrentUser currentUser, IGenericRepository<Role> roleGenericRepository,
            IGenericRepository<Room> roomGenericRepository)
        {
            _currentUser = currentUser;
            _roleGenericRepository = roleGenericRepository;
            _roomGenericRepository = roomGenericRepository;
        }

        public async Task<bool> SetUpRole(Room room, Role role, Dictionary<string, bool?> permissions)
        {
            var user = _currentUser.User;
            var participant = room.Participants.FirstOrDefault(participant => participant.UserId == user.Id);
            var userRole = await _roleGenericRepository.GetEntityById(participant?.RoleId);
            
            if (!userRole.CanManageRoles || room.RoomRolesId.All(roomRoleId => roomRoleId != role.Id))
            {
                return false;
            }
            
            
            
            await _roleGenericRepository.UpdateAsync(role);

            return true;
        }
        
        public async Task<bool> SetUpRole(Room room, Role role, bool? canPin = null, bool? canInvite = null,
            bool? canDeleteOthersMessages = null, bool? canModerateParticipants = null, bool? canManageRoles = null, 
            bool? canManageChannels = null, bool? canManageRoom = null, bool? canUseAdminChannels = null, 
            bool? canViewAuditLog = null)
        {
            var user = _currentUser.User;
            var participant = room.Participants.FirstOrDefault(participant => participant.UserId == user.Id);
            var userRole = await _roleGenericRepository.GetEntityById(participant?.RoleId);

            if (!userRole.CanManageRoles || room.RoomRolesId.All(roomRoleId => roomRoleId != role.Id))
            {
                return false;
            }
            
            if (canPin is { } pin)
            {
                role.CanPin = pin;
            }
            
            if (canInvite is { } invite)
            {
                role.CanInvite = invite;
            }

            if (canDeleteOthersMessages is { } deleteMessages)
            {
                role.CanDeleteOthersMessages = deleteMessages;
            }

            if (canModerateParticipants is { } moderateParticipants)
            {
                role.CanModerateParticipants = moderateParticipants;
            }

            if (canManageRoles is { } manageRoles)
            {
                role.CanManageRoles = manageRoles;
            }

            if (canManageChannels is { } manageChannels)
            {
                role.CanManageChannels = manageChannels;
            }

            if (canManageRoom is { } manageRoom)
            {
                role.CanManageRoom = manageRoom;
            }

            if (canUseAdminChannels is { } useAdminChannels)
            {
                role.CanUseAdminChannels = useAdminChannels;
            }

            if (canViewAuditLog is { } viewAuditLog)
            {
                role.CanViewAuditLog = viewAuditLog;
            }

            await _roleGenericRepository.UpdateAsync(role);

            return true;
        }

        public async Task<bool> CreateNewRole(Room room, string name)
        {
            if (!CanManageRoles(room))
            {
                return false;
            }
            
            var newRole = new Role(name);
            
            room.RoomRolesId.Add(newRole.Id);

            await _roleGenericRepository.CreateAsync(newRole);

            await _roomGenericRepository.UpdateAsync(room);

            return true;
        }

        public async Task<bool> DeleteRole(Room room, Role role)
        {
            if (!CanManageRoles(room) || room.BaseRoleId == role.Id ||
                room.RoomRolesId.Any(roleId => roleId == role.Id))
            {
                return false;
            }

            room.RoomRolesId.Remove(role.Id);

            await _roomGenericRepository.UpdateAsync(room);

            await _roleGenericRepository.DeleteAsync(role);

            return true;
        }

        public async Task<List<Role>> GetAllRolesInRoom(Room room)
        {
            var resultRoles = new List<Role>();

            foreach (var roleId in room.RoomRolesId)
            {
                resultRoles.Add(await _roleGenericRepository.GetEntityById(roleId));
            }

            return resultRoles;
        }

        public async Task<bool> SetRoleToUser(Room room, User user, Role role)
        {
            if (!CanManageRoles(room) || room.RoomRolesId.Any(roleId => roleId == role.Id))
            {
                return false;
            }
            
            var participant = room.Participants.FirstOrDefault(participant => participant.UserId == user.Id);
            
            if (participant == null)
            {
                return false;
            }
            
            participant.RoleId = role.Id;

            await _roomGenericRepository.UpdateAsync(room);
            return true;
        }

        private bool CanManageRoles(Room room)
        {
            var user = _currentUser.User;
            var participant = room.Participants.FirstOrDefault(participant => participant.UserId == user.Id);
            var userRole = _roleGenericRepository.GetEntityById(participant?.RoleId).Result;

            if (userRole.CanManageRoles)
            {
                return true;
            }

            return false;
        }
    }
}

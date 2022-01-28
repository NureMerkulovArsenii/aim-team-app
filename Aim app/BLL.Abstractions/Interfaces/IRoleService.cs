using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IRoleService
    {
        Task<bool> SetUpRole(Room room, Role role, bool? canPin = null, bool? canInvite = null,
            bool? canDeleteOthersMessages = null, bool? canModerateParticipants = null, bool? canManageRoles = null,
            bool? canManageChannels = null, bool? canManageRoom = null, bool? canUseAdminChannels = null,
            bool? canViewAuditLog = null);

        Task<bool> CreateNewRole(Room room, string name);

        Task<bool> DeleteRole(Room room, Role role);

        Task<List<Role>> GetAllRolesInRoom(Room room);
        
        Task<bool> SetRoleToUser(Room room, User user, Role role);
    }
}

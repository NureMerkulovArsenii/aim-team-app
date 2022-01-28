using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IRoleService
    {
        bool SetUpRole(Room room, Role role, bool? canPin = null, bool? canInvite = null,
            bool? canDeleteOthersMessages = null, bool? canModerateParticipants = null, bool? canManageRoles = null,
            bool? canManageChannels = null, bool? canManageRoom = null, bool? canUseAdminChannels = null,
            bool? canViewAuditLog = null);
    }
}

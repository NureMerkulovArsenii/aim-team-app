namespace Core
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; }
        
        public bool CanPin { get; set; }
        
        public bool CanInvite { get; set; }
        
        public bool CanDeleteOthersMessages { get; set; }
        
        public bool CanModerateParticipants { get; set; }
        
        public bool CanManageRoles { get; set; }
        
        public bool CanManageChannels { get; set; }

        public bool CanManageRoom { get; set; }

        public bool CanUseAdminChannels { get; set; }

        public bool CanViewAuditLog { get; set; }
    }
}

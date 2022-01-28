﻿using Newtonsoft.Json.Serialization;

namespace Core
{
    public class Role : BaseEntity
    {
        public Role(string name = "New Role")
        {
            RoleName = name;
            CanPin = false;
            CanInvite = false;
            CanDeleteOthersMessages = false;
            CanModerateParticipants = false;
            CanManageRoles = false;
            CanManageChannels = false;
            CanManageRoom = false;
            CanUseAdminChannels = false;
            CanViewAuditLog = false;
    }
        
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
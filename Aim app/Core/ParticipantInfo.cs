﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ParticipantInfo
    {
        public User User { get; set; }
        
        public string RoleId { get; set; }

        public bool Notifications { get; set; }
    }
}
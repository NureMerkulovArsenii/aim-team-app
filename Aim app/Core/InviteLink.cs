using System.Collections.Generic;

namespace Core
{
    public class InviteLink : BaseEntity
    {
        public string Url { get; set; }
        
        public string ExpirationTime { get; set; }

        public List<User> User { get; set; }

        public Room Room { get; set; }
        
        public bool IsUsed { get; set; }
    }
}

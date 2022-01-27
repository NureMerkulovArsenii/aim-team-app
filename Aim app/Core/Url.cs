using System.Collections.Generic;

namespace Core
{
    public class Urls : BaseEntity
    {
        public string Url { get; set; }
        public string ExpirationTime { get; set; }
        public List<string> UserId { get; set; }
        public string RoomId { get; set; }
        public bool IsUsed { get; set; }
    }
}

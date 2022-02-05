using System.Collections.Generic;

namespace Core
{
    public class PersonalChat : BaseEntity
    {
        public string ChatName { get; set; }

        public IList<UsersPersonalChats> Participants { get; set; }
    }
}

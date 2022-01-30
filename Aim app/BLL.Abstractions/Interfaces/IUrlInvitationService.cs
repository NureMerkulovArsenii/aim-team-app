using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IUrlInvitationService
    {
        Task InviteUsersByEmailWithUrlAsync(Room room, List<string> users);
        
        public Task<bool> JoinByUrl(string url);
        
        string InviteUsersByUrl(Room room);
    }
}

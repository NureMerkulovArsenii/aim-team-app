using System.Threading.Tasks;
using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IUrlInvitationService
    {
        void InviteUsersByEmailWithUrl(Room roomId, string[] users);
        public Task<bool> JoinByUrl(string url);
        string InviteUsersByUrl(Room room);
    }
}

using System.Threading.Tasks;
using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IUrlInvitationService
    {
        void InviteUserByUrl(Room roomId, User userId);
        public Task<bool> JoinByUrl(string url);
        string InviteUsersByUrl(Room roomId);
    }
}

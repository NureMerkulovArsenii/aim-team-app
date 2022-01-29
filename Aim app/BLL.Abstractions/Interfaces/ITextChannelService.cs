using System.Threading.Tasks;
using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface ITextChannelService
    {
        Task<bool> EditTextChannel(TextChannel textChannel, Room room, string name = null, string description = null, bool? isAdmin = false);

        Task<bool> DeleteTextChannel(TextChannel textChannel, Room room);
    }
}

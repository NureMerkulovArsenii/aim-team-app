using System;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class TextChannelService : ITextChannelService
    {
        private readonly IGenericRepository<TextChannel> _textChatGenericRepository;
        private readonly IGenericRepository<Room> _roomGenericRepository;
        private readonly IGenericRepository<Role> _roleGenericRepository;
        private readonly ICurrentUser _currentUser;

        public TextChannelService(IGenericRepository<TextChannel> textChatGenericRepository,
            IGenericRepository<Room> roomGenericRepository,
            IGenericRepository<Role> roleGenericRepository, ICurrentUser currentUser)
        {
            _textChatGenericRepository = textChatGenericRepository;
            _roomGenericRepository = roomGenericRepository;
            _roleGenericRepository = roleGenericRepository;
            _currentUser = currentUser;
        }
        
        public async Task<bool> EditTextChannel(TextChannel textChannel, Room room, string name = null, string description = null, bool? isAdmin = false)
        {
            var user = _currentUser.User;

            if (!await CanManageChannels(room, user) ||
                (!await CanUseAdminChannels(room, user) && isAdmin is false))
            {
                return false;
            }

            try
            {
                if (name != null)
                {
                    textChannel.ChannelName = name;
                }

                if (description != null)
                {
                    textChannel.ChannelDescription = description;
                }
                
                if (isAdmin.HasValue)
                {
                    textChannel.IsAdminChannel = isAdmin.Value;
                }

                await _textChatGenericRepository.UpdateAsync(textChannel);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteTextChannel(TextChannel textChannel, Room room)
        {
            if (!await CanManageChannels(room, _currentUser.User))
            {
                return false;
            }
            
            try
            {
                await _textChatGenericRepository.DeleteAsync(textChannel);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> CanManageChannels(Room room, User user)
        {
            var participant = room.Participants.FirstOrDefault(participant => participant.UserId == user.Id);
            var userRole = await _roleGenericRepository.GetEntityById(participant?.RoleId);

            return userRole.CanManageChannels;
        }

        private async Task<bool> CanUseAdminChannels(Room room, User user)
        {
            var participant = room.Participants.FirstOrDefault(participant => participant.UserId == user.Id);
            var userRole = await _roleGenericRepository.GetEntityById(participant?.RoleId);

            return userRole.CanUseAdminChannels;
        }
    }
}

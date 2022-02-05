using System.Collections.Generic;
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
        private readonly ICurrentUser _currentUser;
        private readonly IGenericRepository<Role> _roleGenericRepository;
        private readonly IGenericRepository<Room> _roomGenericRepository;
        private readonly IGenericRepository<TextChannel> _textChatGenericRepository;

        public TextChannelService(ICurrentUser currentUser, IGenericRepository<Role> roleGenericRepository,
            IGenericRepository<Room> roomGenericRepository, IGenericRepository<TextChannel> textChatGenericRepository)
        {
            _currentUser = currentUser;
            _roleGenericRepository = roleGenericRepository;
            _roomGenericRepository = roomGenericRepository;
            _textChatGenericRepository = textChatGenericRepository;
        }

        public async Task<bool> EditTextChannel(TextChannel textChannel, Room room, string name = null,
            string description = null, bool? isAdmin = null)
        {
            var user = _currentUser.User;
            
            if (!await CanManageChannels(room, user) ||
                (!await CanUseAdminChannels(room, user) && (textChannel.IsAdminChannel || isAdmin is true)))
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

        public async Task<bool> CreateTextChannel(Room room, string name, string description, bool isAdmin = false)
        {
            var user = _currentUser.User;

            if (!await CanManageChannels(room, user) || (isAdmin && !await CanUseAdminChannels(room, user)))
            {
                return false;
            }

            var newTextChannel = new TextChannel()
            {
                ChannelName = name, ChannelDescription = description, IsAdminChannel = isAdmin
            };
            
            room.TextChannels.Add(newTextChannel);

            await _textChatGenericRepository.CreateAsync(newTextChannel);

            await _roomGenericRepository.UpdateAsync(room);

            return true;
        }

        public async Task<List<TextChannel>> GetTextChannels(Room room)
        {
            var result = new List<TextChannel>();
            var user = _currentUser.User;

            foreach (var chat in room.TextChannels)
            {
                if ((chat.IsAdminChannel && await CanUseAdminChannels(room, user)) || !chat.IsAdminChannel)
                {
                    result.Add(chat);
                }
            }

            return result;
        }

        public async Task<bool> DeleteTextChannel(TextChannel textChannel, Room room)
        {
            if (!await CanManageChannels(room, _currentUser.User))
            {
                return false;
            }
            
            room.TextChannels.Remove(textChannel);

            try
            {
                await _textChatGenericRepository.DeleteAsync(textChannel);
                await _roomGenericRepository.UpdateAsync(room);
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CanManageChannels(Room room, User user = null)
        {
            if (user == null)
            {
                user = _currentUser.User;
            }

            var participant = room.Participants.FirstOrDefault(participant => participant.User.Id == user.Id);
            var userRole = participant?.Role;

            return userRole!.CanManageChannels;
        }

        public async Task<bool> CanUseAdminChannels(Room room, User user = null)
        {
            if (user == null)
            {
                user = _currentUser.User;
            }

            var participant = room.Participants.FirstOrDefault(participant => participant.User.Id == user.Id);
            var userRole = participant?.Role;

            return userRole!.CanUseAdminChannels;
        }
    }
}

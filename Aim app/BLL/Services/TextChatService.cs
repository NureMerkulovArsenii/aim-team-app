using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class TextChatService : ITextChatService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IGenericRepository<Role> _roleGenericRepository;
        private readonly IGenericRepository<Room> _roomGenericRepository;
        private readonly IGenericRepository<TextChat> _textChatGenericRepository;

        public TextChatService(ICurrentUser currentUser, IGenericRepository<Role> roleGenericRepository,
            IGenericRepository<Room> roomGenericRepository, IGenericRepository<TextChat> textChatGenericRepository)
        {
            _currentUser = currentUser;
            _roleGenericRepository = roleGenericRepository;
            _roomGenericRepository = roomGenericRepository;
            _textChatGenericRepository = textChatGenericRepository;
        }

        public async Task<bool> CreateTextChat(Room room, string name, string description, bool isAdmin=false)
        {
            var user = _currentUser.User;

            if (!CanManageChannels(room, user) || (isAdmin && !CanUseAdminChannels(room, user)))
            {
                return false;
            }

            var newTextChat = new TextChat()
            {
                ChatName = name,
                ChatDescription = description,
                IsAdmin = isAdmin
            };
            
            room.TextChatsId.Add(newTextChat.Id);

            await _textChatGenericRepository.CreateAsync(newTextChat);
            
            await _roomGenericRepository.UpdateAsync(room);
            
            return true;
        }

        private bool CanManageChannels(Room room, User user)
        {
            var participant = room.Participants.FirstOrDefault(participant => participant.UserId == user.Id);
            var userRole = _roleGenericRepository.GetEntityById(participant?.RoleId).Result;

            if (userRole.CanManageChannels)
            {
                return true;
            }

            return false;
        }

        private bool CanUseAdminChannels(Room room, User user)
        {
            var participant = room.Participants.FirstOrDefault(participant => participant.UserId == user.Id);
            var userRole = _roleGenericRepository.GetEntityById(participant?.RoleId).Result;

            if (userRole.CanUseAdminChannels)
            {
                return true;
            }

            return false;
        }
    }
}

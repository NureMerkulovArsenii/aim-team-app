using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class PersonalChatService : IPersonalChatService
    {
        private readonly IGenericRepository<PersonalChat> _genericRepositoryChat;
        private readonly IGenericRepository<User> _genericRepositoryUser;
        private readonly ICurrentUser _currentUser;

        public PersonalChatService(IGenericRepository<PersonalChat> genericRepository, IGenericRepository<User> genericRepositoryUser, ICurrentUser currentUser)
        {
            this._genericRepositoryChat = genericRepository;
            this._genericRepositoryUser = genericRepositoryUser;
            this._currentUser = currentUser;
        }

        public async Task CreatePersonalChat(string userToChatWith) // add abstraction
        {
            var participant = _genericRepositoryUser.FindByConditionAsync(user =>
                user.UserName == userToChatWith || user.Email == userToChatWith).Result.FirstOrDefault();
            
            var participants = new List<string>
            {
                _currentUser.User.Id,
                participant.Id
            };

            var personalChat = new PersonalChat()
            {
                ChatName = userToChatWith, 
                ParticipantsIds = participants
            };

            await _genericRepositoryChat.CreateAsync(personalChat);
        }
        
        public 

        public async Task AddParticipants(PersonalChat chat)
        {
            
        }
    }
}

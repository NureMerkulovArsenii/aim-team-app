using System;
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

        public PersonalChatService(IGenericRepository<PersonalChat> genericRepository,
            IGenericRepository<User> genericRepositoryUser, ICurrentUser currentUser)
        {
            this._genericRepositoryChat = genericRepository;
            this._genericRepositoryUser = genericRepositoryUser;
            this._currentUser = currentUser;
        }

        public async Task CreatePersonalChat(List<string> userToChatWith)
        {
            var participants = new List<string> {_currentUser.User.Id};

            foreach (var userInPersonalChat in userToChatWith)
            {
                var users = await _genericRepositoryUser.FindByConditionAsync(user =>
                    user.UserName == userInPersonalChat || user.Email == userInPersonalChat);
                var user = users.FirstOrDefault();
                if (user != null && !participants.Contains(user.Id))
                {
                    participants.Add(user.Id);
                }
            }

            var chatName = string.Empty;

            for (var i = 0; i < userToChatWith.Count && i < 5; i++)
            {
                chatName += userToChatWith[i];
                if (i != 4 && i != userToChatWith.Count - 1)
                {
                    chatName += ",";
                }
            }

            var personalChat = new PersonalChat() {ChatName = chatName, ParticipantsIds = participants};

            await _genericRepositoryChat.CreateAsync(personalChat);
        }

        public async Task<bool> ChangeNameOfPersonalChat(PersonalChat chat, string name)
        {
            chat.ChatName = name;
            await _genericRepositoryChat.UpdateAsync(chat);
            return true;
        }

        public async Task AddParticipantsToPersonalChat(PersonalChat chat, string[] participants)
        {
            try
            {
                foreach (var participant in participants)
                {
                    var user = await _genericRepositoryUser.FindByConditionAsync(user =>
                        user.UserName == participant || user.Email == participant);
                    if (!chat.ParticipantsIds.Contains(user.FirstOrDefault().Id))
                    {
                        chat.ParticipantsIds.Add(user.FirstOrDefault().Id);
                    }
                }

                await _genericRepositoryChat.UpdateAsync(chat);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<IList<PersonalChat>> GetUserPersonalChats()
        {
            var userChats = await
                _genericRepositoryChat.FindByConditionAsync(chat =>
                    chat.ParticipantsIds.Contains(_currentUser.User.Id));


            return userChats.ToList();
        }

        public async Task<IList<string>> GetUserNamesOfChat(PersonalChat chat)
        {
            var users = new List<string>();
            var ids = chat.ParticipantsIds;

            foreach (var id in ids)
            {
                var participants = await _genericRepositoryUser.FindByConditionAsync(user => user.Id == id);
                users.Add(participants.FirstOrDefault()?.UserName);
            }

            return users;
        }

        public async Task<bool> LeavePersonalChat(PersonalChat chat)
        {
            var user = _currentUser.User;

            if (!chat.ParticipantsIds.Remove(user.Id))
            {
                return false;
            }

            if (chat.ParticipantsIds.Count == 0)
            {
                await _genericRepositoryChat.DeleteAsync(chat);
                return true;
            }

            await _genericRepositoryChat.UpdateAsync(chat);

            return true;
        }
    }
}

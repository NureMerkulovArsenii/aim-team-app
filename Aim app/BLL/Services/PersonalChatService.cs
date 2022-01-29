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

        public async Task CreatePersonalChat(string[] usersToChat) // add abstraction
        {
            var participants = new List<string> {_currentUser.User.Id};

            foreach (var userInPersonalChat in usersToChat)
            {
                var userId = _genericRepositoryUser.FindByConditionAsync(user =>
                    user.UserName == userInPersonalChat || user.Email == userInPersonalChat).Result.FirstOrDefault();
                participants.Add(userId.Id);
            }

            var chatName = string.Empty;

            for (var i = 0; i < usersToChat.Length && i < 5; i++)
            {
                chatName += usersToChat[i];
                if (i != 4 && i != usersToChat.Length - 1)
                {
                    chatName += ",";
                }
            }

            var personalChat = new PersonalChat() {ChatName = chatName, ParticipantsIds = participants};

            await _genericRepositoryChat.CreateAsync(personalChat);
        }

        public async Task<bool> ChangeNameOfPersonalChat(PersonalChat chat, string name)
        {
            try
            {
                chat.ChatName = name;
                return true;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task AddParticipantsToPersonalChat(PersonalChat chat, string[] participants)
        {
            try
            {

                foreach (var participant in participants)
                {
                    var users = await _genericRepositoryUser.FindByConditionAsync(user =>
                        user.UserName == participant || user.Email == participant);
                    chat.ParticipantsIds.Add(users.FirstOrDefault().Id);
                }
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

            //var result = userChats.Select(chat => chat.Id).ToList();

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

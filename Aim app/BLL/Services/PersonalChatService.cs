using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class PersonalChatService : IPersonalChatService
    {
        private readonly IGenericRepository<PersonalChat> _genericRepositoryChat;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public PersonalChatService(IGenericRepository<PersonalChat> genericRepository,
            ICurrentUser currentUser, IUnitOfWork unitOfWork)
        {
            this._genericRepositoryChat = genericRepository;
            this._currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task CreatePersonalChat(List<string> userToChatWith)
        {
            var participants = new List<User> {_currentUser.User};

            foreach (var userInPersonalChat in userToChatWith)
            {
                var users = await _unitOfWork.UserRepository.FindByConditionAsync(user =>
                    user.UserName == userInPersonalChat || user.Email == userInPersonalChat);
                var user = users.FirstOrDefault();
                if (user != null && !participants.Select(x => x.Id).Contains(user.Id))
                {
                    participants.Add(user);
                }
            }

            if (participants.Count == 1)
            {
                return;
            }

            var chatName = GenerateChatName(userToChatWith);

            var personalChat = new PersonalChat {ChatName = chatName};

            var usersOfPersonalChat = new List<UsersPersonalChats>();
            await _unitOfWork.CreateTransactionAsync();

            foreach (var participant in participants)
            {
                var userOfPersonalChat = new UsersPersonalChats() {User = participant, PersonalChat = personalChat};

                await _unitOfWork.UsersPersonalChats.CreateAsync(userOfPersonalChat);
                await _unitOfWork.SaveAsync();

                usersOfPersonalChat.Add(userOfPersonalChat);
            }

            personalChat.Participants = usersOfPersonalChat;

            await _unitOfWork.PersonalChatRepository.CreateAsync(personalChat);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> ChangeNameOfPersonalChat(PersonalChat chat, string name)
        {
            chat.ChatName = name;
            await _unitOfWork.PersonalChatRepository.UpdateAsync(chat);
            return true;
        }

        public async Task AddParticipantsToPersonalChat(PersonalChat chat, List<string> participants)
        {
            try
            {
                await _unitOfWork.CreateTransactionAsync();

                foreach (var participant in participants)
                {
                    var usersOfPersonalChats = new UsersPersonalChats();
                    var user = (await _unitOfWork.UserRepository.FindByConditionAsync(user =>
                        user.UserName == participant || user.Email == participant)).FirstOrDefault();

                    if (!chat.Participants.Select(x => x.User.Id).Contains(user.Id))
                    {
                        usersOfPersonalChats.PersonalChat = chat;
                        usersOfPersonalChats.User = user;
                        await _unitOfWork.UsersPersonalChats.UpdateAsync(usersOfPersonalChats);
                        await _unitOfWork.SaveAsync();
                    }
                }

                await _unitOfWork.PersonalChatRepository.UpdateAsync(chat);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (NullReferenceException e)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine(e);
            }
        }

        public async Task<IList<PersonalChat>> GetUserPersonalChats()
        {
            var userChats = await
                _unitOfWork.PersonalChatRepository.FindByConditionAsync(chat =>
                    chat.Participants.Select(x => x.Id).Contains(_currentUser.User.Id));

            return userChats.ToList();
        }

        public async Task<IList<string>> GetUserNamesOfChat(PersonalChat chat)
        {
            var users = chat.Participants.Select(user => user.User);

            return users.Select(user => user.UserName).ToList();
        }

        public async Task<bool> LeavePersonalChat(PersonalChat chat)
        {
            var user = _currentUser.User;
            var participant = chat.Participants.FirstOrDefault(chats => chats.User.Id == user.Id);

            await _unitOfWork.CreateTransactionAsync();

            if (!chat.Participants.Remove(participant))
            {
                return false;
            }

            if (chat.Participants.Count == 0)
            {
                await _genericRepositoryChat.DeleteAsync(chat);
                return true;
            }

            await _unitOfWork.PersonalChatRepository.UpdateAsync(chat);
            await _unitOfWork.SaveAsync();
            await _unitOfWork.UsersPersonalChats.DeleteAsync(participant);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.CommitAsync();

            return true;
        }

        private string GenerateChatName(List<string> userNames)
        {
            var chatName = new StringBuilder();

            for (var i = 0; i < userNames.Count && i < 5; i++)
            {
                chatName.Append(userNames[i]);
                if (i != 4 && i != userNames.Count - 1)
                {
                    chatName.Append(',');
                }
            }

            return chatName.ToString();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class PersonalChatService : IPersonalChatService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public PersonalChatService(ICurrentUser currentUser, IUnitOfWork unitOfWork)
        {
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

            var personalChat = new PersonalChat {ChatName = chatName, Participants = new List<UsersPersonalChats>()};


            await _unitOfWork.CreateTransactionAsync();

            foreach (var participant in participants)
            {
                var userOfPersonalChat = new UsersPersonalChats() {User = participant, PersonalChat = personalChat};
                personalChat.Participants.Add(userOfPersonalChat);
            }

            await _unitOfWork.PersonalChatRepository.CreateAsync(personalChat);
            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitAsync();

            // foreach (var userOfPersonalChat in personalChat.Participants)
            // {
            //     await _unitOfWork.UsersPersonalChats.CreateAsync(userOfPersonalChat);
            //     await _unitOfWork.SaveAsync();
            // }


            //await _unitOfWork.PersonalChatRepository.CreateAsync(personalChat);
            //await _unitOfWork.SaveAsync();

            // await _unitOfWork.CommitAsync();
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
            // var users = await
            //     _unitOfWork.PersonalChatRepository.FindByConditionAsync(chat => chat.Participants
            //         .Select(x => x.User.Id)
            //         .Contains(_currentUser.User.Id));


            var users = await _unitOfWork.PersonalChatRepository.FindByConditionAsync(q => q.Participants
                .Select(chats => chats.User.Id)
                .Contains(_currentUser.User.Id), PersonalChat.Selector);

            return users.ToList();
        }

        public async Task<IList<string>> GetUserNamesOfChat(PersonalChat chat)
        {
            var users = chat.Participants.Select(user => user.User);

            return users.Select(user => user.UserName).ToList();
        }

        public async Task<bool> LeavePersonalChat(PersonalChat chat)
        {
            var user = _currentUser.User;
            //var participant = chat.Participants.FirstOrDefault(chats => chats.User.Id == user.Id);
            var participant = chat.Participants.First(personalChats => personalChats.User.Id == user.Id);

            await _unitOfWork.CreateTransactionAsync();

            if (!chat.Participants.Remove(participant))
            {
                return false;
            }

            if (chat.Participants.Count == 0)
            {
                await _unitOfWork.PersonalChatRepository.DeleteAsync(chat);
                return true;
            }

            await _unitOfWork.UsersPersonalChats.DeleteAsync(participant);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.PersonalChatRepository.UpdateAsync(chat);
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

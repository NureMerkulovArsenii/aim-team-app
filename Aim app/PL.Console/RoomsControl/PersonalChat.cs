using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using PL.Console.Interfaces;

namespace PL.Console.RoomsControl
{
    public class PersonalChat : IPersonalChat
    {
        private readonly IPersonalChatService _chatService;
        private readonly IUserValidator _userValidator;
        private readonly ICurrentUser _currentUser;

        public PersonalChat(IPersonalChatService chatService, IUserValidator userValidator, ICurrentUser currentUser)
        {
            this._chatService = chatService;
            this._userValidator = userValidator;
            this._currentUser = currentUser;
        }

        public void StartChat()
        {
            System.Console.WriteLine(
                "Enter username / email that you want to invite(if many users enumerate them with \"space\")");
            var userNames = System.Console.ReadLine().Split(" ");
            var usersToInvite = new List<string>();
            var errors = new List<string>();
            foreach (var name in userNames)
            {
                var validationsResult = _userValidator.ValidateUserNameOrEmail(name);
                if (validationsResult == true)
                {
                    usersToInvite.Add(name);
                }
                else
                {
                    errors.Add(name);
                }
            }

            if (errors.Any())
            {
                System.Console.WriteLine("Such users have not been added to group chat:");
                foreach (var error in errors)
                {
                    System.Console.WriteLine(error);
                }
            }

            // System.Console.WriteLine($"{userName}\n start typing:");
            //here must be called message worker or smth like this
            //when first message is sent, then chat is initiated

            //on message sent:
<<<<<<< HEAD
            _chatService.CreatePersonalChat(userName);
            
            

=======
            _chatService.CreatePersonalChat(usersToInvite.ToArray());
        }

        public async Task GetUserPersonalChats()
        {
            var chats = await _chatService.GetUserPersonalChats();

            if (!chats.Any())
            {
                System.Console.WriteLine("You have no chats");
            }
            else
            {
                System.Console.WriteLine("Your chats: ");

                for (var i = 0; i < chats.Count; i++)
                {
                    System.Console.WriteLine($"\t0{i + 1}) {chats[i]}");
                }
            }
            
        }

        public void DoAction(PersonalChat chat)
        {
            System.Console.WriteLine("You successfully entered chat!");
>>>>>>> origin/personal-chats
        }
    }
}

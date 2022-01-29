using BLL.Abstractions.Interfaces;

namespace PL.Console.RoomsControl
{
    public class PersonalChat
    {
        private readonly IPersonalChatService _chatService;
        private readonly IUserValidator _userValidator;

        public PersonalChat(IPersonalChatService chatService, IUserValidator userValidator)
        {
            this._chatService = chatService;
            this._userValidator = userValidator;
        }
        
        public void StartChat()
        {
            System.Console.WriteLine("Enter users nickname/email to start chat:");
            var userName = System.Console.ReadLine();
            while (!_userValidator.ValidateUserNameOrEmail(userName))
            {
                System.Console.WriteLine("User with such nickname doesnt exist\n Try again:");
                userName = System.Console.ReadLine();
            }

            System.Console.WriteLine($"{userName}\n start typing:");
            //here must be called message worker or smth like this
            //when first message is sent, then chat is initiated

            //on message sent:
            _chatService.CreatePersonalChat();
            
            

        }
    }
}

using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using PL.Console.Interfaces;

namespace PL.Console.Authorization
{
    public class Authorization : IAuthorization
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IMailWorker _mailWorker;
        private readonly ICurrentUser _currentUser;

        public Authorization(IAuthorizationService authorizationService, IMailWorker mailWorker, ICurrentUser currentUser)
        {
            _authorizationService = authorizationService;
            _mailWorker = mailWorker;
            _currentUser = currentUser;
        }
        
        public async Task<bool> AuthorizeUserAsync()
        {
            System.Console.Write("Enter your username/email: ");
            var usernameOrEmail = System.Console.ReadLine()?.Trim();

            System.Console.Write("Enter your password: ");
            var password = System.Console.ReadLine()?.Trim();

            var isUserDataValid = await _authorizationService.CheckUserDataForAuthAsync(usernameOrEmail, password);
            
            while (!isUserDataValid)
            {
                System.Console.WriteLine("Invalid username or password");
                
                System.Console.Write("Enter your username/email: ");
                usernameOrEmail = System.Console.ReadLine()?.Trim();
                
                System.Console.Write("Enter your password: ");
                password = System.Console.ReadLine()?.Trim();
                
                isUserDataValid = await _authorizationService.CheckUserDataForAuthAsync(usernameOrEmail, password);
            }

            var tempUser = await _authorizationService.GetInfoAboutUser(usernameOrEmail);

            if (await _authorizationService.IsLastAuthWasLongAgo(tempUser, 10))
            {
                var email = await _authorizationService.GetEmailByUsernameOrEmail(usernameOrEmail);
                var code = await _mailWorker.SendCodeByEmailAsync(email);
            
                System.Console.Write($"Enter code from message sent on your email ({email}): ");
                var codeFromUser = System.Console.ReadLine()?.Trim();

                while (codeFromUser != code)
                {
                    System.Console.Write("Wrong code! Enter code from message sent on your email or \"r\" to resend code: ");
                    codeFromUser =  System.Console.ReadLine()?.Trim();

                    if (codeFromUser == "r")
                    {
                        await _mailWorker.SendCodeByEmailAsync(email, code);
                    }
                }
            }

            _currentUser.User = tempUser;
            await _authorizationService.UpdateLastAuth(tempUser);
            return true;
        }
    }
}

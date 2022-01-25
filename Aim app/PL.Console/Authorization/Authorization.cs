using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;

namespace PL.Console.Authorization
{
    public class Authorization
    {
        private readonly IAuthorizationService _authorizationService;

        public Authorization(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        
        public async Task<bool> AuthorizeUserAsync()
        {
            System.Console.Write("Enter your login/email: ");
            var loginOrEmail = System.Console.ReadLine()?.Trim();

            System.Console.Write("Enter your password: ");
            var password = System.Console.ReadLine()?.Trim();
        
            if (await _authorizationService.CheckUserDataForLoginAsync(loginOrEmail, password))
            {
                // Берем из БД email
                var email = "zhmaytsevm@gmail.com";
                // var code = await _authorizationService.SendCodeByEmailAsync(email);
                await _authorizationService.SendCodeByEmailAsync(email);
                
                System.Console.Write("Enter code from message sent on your email: ");
                var codeFromUser = System.Console.ReadLine()?.Trim();

                while (!_authorizationService.CompareCodes(codeFromUser))
                {
                    System.Console.Write("Wrong code! Enter code from message sent on your email: ");
                    codeFromUser =  System.Console.ReadLine()?.Trim();
                }

                // while (!_authorizationService.CompareCodes(codeFromUser))
                // {
                //     
                //     System.Console.Write("Enter code from message sent on your email (\"r\" to resend code): ");
                //     var codeFromUser = System.Console.ReadLine()?.Trim();
                //     
                //     if (codeFromUser == code)
                //     {
                //         
                //     }
                //     else
                //     {
                //         System.Console.WriteLine("Wrong code!");
                //     }
                // }
                return true;
            }
            else
            {
                System.Console.WriteLine("Invalid login or password");
                return false;
            }
        }
    }
}

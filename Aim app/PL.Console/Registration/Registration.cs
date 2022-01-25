using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;

namespace PL.Console.Registration
{
    public class Registration
    {
        private readonly IRegistrationService _registrationService;

        public Registration(IRegistrationService registrationService)
        {
            this._registrationService = registrationService;
        }

        public async Task RegisterUserAsync()
        {
            System.Console.WriteLine("Enter your email");
            var email = System.Console.ReadLine();
            System.Console.WriteLine("Enter your password");
            var password = _registrationService.GetHashFromString(System.Console.ReadLine());

            await _registrationService.RegisterAsync(email, password);

            System.Console.WriteLine("Check your email and enter code:");

            var code = System.Console.ReadLine();
            while (!_registrationService.CompareCodes(code))
            {
                System.Console.WriteLine("Wrong code");
                System.Console.WriteLine(
                    "Enter \"y\" if you want to reenter code from email and \"n\" if you want to send it again");
                var userAnswer = System.Console.ReadLine();
                if (userAnswer == "n")
                {
                    await _registrationService.RegisterAsync(email, password);
                    System.Console.WriteLine("Check your email and enter code:");
                    code = System.Console.ReadLine();
                }

                if (userAnswer == "y")
                {
                    code = System.Console.ReadLine();
                }
            }
        }
    }
}

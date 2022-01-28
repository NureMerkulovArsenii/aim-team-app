using BLL.Abstractions.Interfaces;
using PL.Console.Interfaces;

namespace PL.Console.RoomsControl
{
    public class UserControl : IUserControl
    {
        private readonly IPasswordService _passwordService;
        private readonly IUserService _userService;

        public UserControl(IPasswordService passwordService, IUserService userService)
        {
            _passwordService = passwordService;
            _userService = userService;
        }
        
        public bool ChooseAction()
        {
            string action;
            do
            {
                System.Console.Write("What do you want to change? (\"password\" or \"name\")");
                action = System.Console.ReadLine();
            } while (action != "password" && action != "name");

            if (action == "password")
            {
                return ChangePassword();
            }

            if (action == "name")
            {
                ChangeNames();
                return true;
            }

            return false;
        }

        private bool ChangePassword()
        {
            string oldPass;
            do
            {
                System.Console.Write("Enter your old password: ");
                oldPass = System.Console.ReadLine();
            } while (oldPass == null);
            
            string newPass;
            do
            {
                System.Console.Write("Enter your old password: ");
                newPass = System.Console.ReadLine();
            } while (newPass == null);

            return _passwordService.ChangePassword(oldPass, newPass);
        }

        private void ChangeNames()
        {
            string firstNameChanging;
            do
            {
                System.Console.Write("Do you want to change your first name: ");
                firstNameChanging = System.Console.ReadLine();
            } while (firstNameChanging != "yes" || firstNameChanging != "no");
            
            string lastNameChanging;
            do
            {
                System.Console.Write("Do you want to change your last name: ");
                lastNameChanging = System.Console.ReadLine();
            } while (lastNameChanging != "yes" && lastNameChanging != "no");

            string firstName = null;
            string lastName = null;

            if (firstNameChanging != "no")
            {
                do
                {
                    System.Console.Write("Enter your new first name: ");
                    firstName = System.Console.ReadLine();
                } while (firstName == null);
            }
            
            if (lastNameChanging != "no")
            {
                do
                {
                    System.Console.Write("Enter your new last name: ");
                    lastName = System.Console.ReadLine();
                } while (lastName == null);
            }

            _userService.ChangeUserNames(firstName, lastName);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using BLL.Abstractions.Interfaces;
using Core;
using PL.Console.Interfaces;

namespace PL.Console.RoomsControl
{
    public class Invitation : IInvitation
    {
        private readonly IUrlInvitationService _invitationService;
        private readonly IUserValidator _userValidator;
        private readonly ICurrentUser _currentUser;

        public Invitation(IUrlInvitationService invitationService, IUserValidator userValidator,
            ICurrentUser currentUser)
        {
            this._invitationService = invitationService;
            this._userValidator = userValidator;
            this._currentUser = currentUser;
        }

        public void EnterRoomWithUrl()
        {
            System.Console.WriteLine("Enter invitation url: ");
            var url = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (_invitationService.JoinByUrl(url).Result)
                {
                    System.Console.WriteLine("You successfully entered the room");
                }
                else
                {
                    System.Console.WriteLine("You dont have access try again later");
                }
            }
            else
            {
                System.Console.WriteLine("Something went wrong, try later");
            }
        }

        public void InviteToRoomWithUrl(Room room)
        {
            System.Console.WriteLine(
                "If you want invite some users press \"s\", if you want to invite 1 specified user press \"o\"");
            var userKey = System.Console.ReadLine();
            while (userKey != "o" && userKey != "s")
            {
                System.Console.WriteLine("Unknown key, please enter again");
                userKey = System.Console.ReadLine();
            }

            if (userKey == "s")
            {
                var url = _invitationService.InviteUsersByUrl(room); //TODO add dest room: done
                System.Console.WriteLine(url);
            }

            if (userKey == "o")
            {
                System.Console.WriteLine(
                    "Enter username / email that you want to invite(if many users enumerate them with \"space\")");
                var userNames = System.Console.ReadLine().Split(" ");
                var usersToInvite = new List<string>();
                var errors = new List<string>();
                foreach (var name in userNames)
                {
                    var validationsResult = _userValidator.ValidateUserNick(name);
                    if (validationsResult == true)
                    {
                        usersToInvite.Add(name);
                    }
                    else
                    {
                        errors.Add(name);
                    }
                }
                //var validationsResult = _userValidator.ValidateUserNick(userName);

                // while (!validationsResult)
                // {
                //     System.Console.WriteLine("User with such credentials does not exist, try again");
                //     userName = System.Console.ReadLine();
                //     validationsResult = _userValidator.ValidateUserNick(userName);
                // }

                _invitationService.InviteUsersByEmailWithUrl(room, userNames);

                if (errors.Any())
                {
                    System.Console.WriteLine("Invitations have not been send to such users:");
                    foreach (var error in errors)
                    {
                        System.Console.WriteLine(error);
                    }
                }
            }
        }
    }
}

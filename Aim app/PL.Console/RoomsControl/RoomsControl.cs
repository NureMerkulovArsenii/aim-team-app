using System.Threading.Tasks;
using Core;
using BLL.Abstractions.Interfaces;
using PL.Console.Interfaces;

namespace PL.Console.RoomsControl
{
    public class RoomsControl : IRoomsControl
    {
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly IInvitation _invitation;
        private readonly IRoleControl _roleControl;

        public RoomsControl(IRoomService roomService, IUserService userService, IInvitation invitation, IRoleControl roleControl)
        {
            this._roomService = roomService;
            this._userService = userService;
            this._invitation = invitation;
            this._roleControl = roleControl;
        }

        public async Task ShowUserRooms()
        {
            var userRooms = await _userService.GetUserRooms();

            if (userRooms.Count == 0)
            {
                System.Console.WriteLine("You have no rooms");
            }
            else
            {
                System.Console.WriteLine("Your rooms: ");

                for (var i = 0; i < userRooms.Count; i++)
                {
                    System.Console.WriteLine($"\t{i + 1}) {userRooms[i].RoomName}");
                }
            }
            
            string userInput;
            do
            {
                System.Console.Write(
                    "If you want to choose room - type its number or if you want to create room - enter \"create\" or if you want to join - enter \"join\": ");
                userInput = System.Console.ReadLine()?.Trim();
            } while (userInput == null && userInput != "create" && userInput != "join" && !int.TryParse(userInput, out var roomNumber));

            
            if (userInput == "create")
            {
                CreateRoom();
            }
            else if (userInput == "join")
            {
                _invitation.EnterRoomWithUrl();
            }
            else if (int.TryParse(userInput, out var roomNumber) && userRooms.Count >= roomNumber)
            {
                ChooseRoomAction(userRooms[roomNumber - 1]);
            }
            else
            {
                System.Console.WriteLine("Error! Please, try again later!");
            }
        }
        
        public bool ChooseRoomAction(Room room)
        {
            string action;
            do
            {
                System.Console.WriteLine("What do you want to do? (\"delete\" or \"set up\" or \"leave\" or \"notification\" or \"invite\" or \"roles\")");
                action = System.Console.ReadLine();
            } while (action == null);

            switch (action)
            {
                case "delete":
                    return DeleteRoom(room);
                    break;
                case "set up":
                    return SetUpRoom(room);
                    break;
                case "leave":
                    return LeaveRoom(room);
                    break;
                case "notification":
                    return ChangeRoomNotifications(room);
                    break;
                case "invite":
                    _invitation.InviteToRoomWithUrl(room);
                    return true;
                    break;
                case "roles":
                    _roleControl.ViewRolesInTheRoom(room);
                    return true;
                    break;
            }

            System.Console.WriteLine("Error! Please, try again later!");

            return false;
        }

        private bool CreateRoom()
        {
            string name;

            do
            {
                System.Console.WriteLine("Enter room's name:");
                name = System.Console.ReadLine();
            } while (name == null);

            System.Console.WriteLine("Enter room's description:");
            var description = System.Console.ReadLine();

            var roomId = _roomService.CreateRoom(name, description);

            if (roomId != null)
            {
                System.Console.WriteLine($"Room successfully created! Its Id is: {roomId}");

                return true;
            }

            System.Console.WriteLine("Error! Please, try again later!");

            return false;
        }

        private bool DeleteRoom(Room room)
        {
            if (_roomService.DeleteRoom(room))
            {
                System.Console.WriteLine("Room successfully deleted!");
                return true;
            }

            System.Console.WriteLine("Error! Please, try again later!");

            return false;
        }

        private bool SetUpRoom(Room room)
        {
            System.Console.WriteLine("What do you want to change? (\"name\" or \"description\" or \"both\")"); // name || description || both
            var action = System.Console.ReadLine();

            string name = null;
            string description = null;

            if (action == "name" || action == "both")
            {
                System.Console.WriteLine("Enter new name:");
                name = System.Console.ReadLine();
            }
            
            if (action == "description" || action == "both")
            {
                System.Console.WriteLine("Enter new description:");
                description = System.Console.ReadLine();
            }

            if (_roomService.ChangeRoomSettings(room, name, description))
            {
                System.Console.WriteLine("Room settings successfully changed!");
                return true;
            }

            System.Console.WriteLine("Error! Please, try again later!");

            return false;
        }

        private bool LeaveRoom(Room room)
        {
            if (_userService.LeaveRoom(room).Result)
            {
                System.Console.WriteLine("Successfully leaved the room!");
                return true;
            }

            return false;
        }

        private bool ChangeRoomNotifications(Room room)
        {
            string userResponse;
            do
            {
                System.Console.Write("Do you want to receive notifications from this room (\"yes\" or \"no\")? ");
                userResponse = System.Console.ReadLine();
            } while (userResponse == null && userResponse != "yes" && userResponse != "no");

            if (userResponse == "yes")
            {
                return _userService.SwitchNotifications(room, true).Result;
            }
            else if (userResponse == "no")
            {
                return _userService.SwitchNotifications(room, false).Result;
            }

            System.Console.WriteLine("Error! Please, try again later!");
            return false;
        }
    }
}

using Core;
using BLL.Abstractions.Interfaces;
using PL.Console.Interfaces;

namespace PL.Console.RoomsControl
{
    public class RoomsControl : IRoomsControl
    {
        private readonly IRoomService _roomService;

        public RoomsControl(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public bool ChooseRoomAction()
        {
            string action;
            do
            {
                System.Console.WriteLine("What do you want to do?");
                action = System.Console.ReadLine();
            } while (action == null);

            if (action == "create")
            {
                return CreateRoom();
            }
            else if (action == "delete")
            {
                return DeleteRoom();
            }
            else if (action == "set up")
            {
                return SetUpRoom();
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

        private bool DeleteRoom()
        {
            System.Console.WriteLine("Choose room:");
            Room room = null; //TODO: Get room

            if (_roomService.DeleteRoom(room))
            {
                System.Console.WriteLine("Room successfully deleted!");
                return true;
            }

            System.Console.WriteLine("Error! Please, try again later!");

            return false;
        }

        private bool SetUpRoom()
        {
            System.Console.WriteLine("Choose room:");
            Room room = null; //TODO: Get room

            System.Console.WriteLine("What do you want to change?"); // name || description || both
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
    }
}

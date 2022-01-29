using System.Linq;
using BLL.Abstractions.Interfaces;
using Core;
using PL.Console.Interfaces;

namespace PL.Console.RoomsControl
{
    public class TextChannelControl : ITextChannelControl
    {
        private readonly ITextChannelService _textChannelService;

        public TextChannelControl(ITextChannelService textChannelService)
        {
            _textChannelService = textChannelService;
        }

        public bool TextChannelChoice(Room room)
        {
            var channelList = _textChannelService.GetTextChannels(room).Result;
            var counter = 1;

            System.Console.WriteLine($"Text channels in {room.RoomName}:");
            
            foreach (var channel in channelList)
            {
                System.Console.WriteLine($" {counter}) {channel.ChannelName}");
                counter++;
            }

            var actions = new string[] {"create"};
            
            System.Console.Write($"What do you want to do? ({string.Join("or", actions)})");
            string action;
            var outAction = -1;
            do
            {
                action = System.Console.ReadLine();
                int.TryParse(action, out outAction);
            } while (action == null || (!actions.Contains(action) && outAction > channelList.Count && outAction < 0));

            if (action == "create")
            {
                if (CreateTextChannel(room))
                {
                    System.Console.WriteLine("Text channel created successfully!");
                    return true;
                }
                
                System.Console.WriteLine("Error! Please, try again later!");

                return false;
            }

            return ChannelsAction(room, channelList[outAction - 1]); //TODO: text channel logic
            }

        private bool CreateTextChannel(Room room)
        {
            if (!_textChannelService.CanManageChannels(room).Result)
            {
                return false;
            }
            
            System.Console.Write("Enter text channel name: ");
            string name;
            do
            {
                name = System.Console.ReadLine();
            } while (name == null);
                
            System.Console.Write("Enter text channel description: ");
            string description;
            do
            {
                description = System.Console.ReadLine();
            } while (description == null);

            var isAdmin = false;
            if (_textChannelService.CanUseAdminChannels(room).Result)
            {
                System.Console.Write("Do you want to this channel be private (\"t\" or \"f\")? ");
                string admin;
                do
                {
                    admin = System.Console.ReadLine();
                } while (admin != "t" && admin != "f");

                if (admin == "t")
                {
                    isAdmin = true;
                }
            }

            return _textChannelService.CreateTextChannel(room, name, description, isAdmin).Result;
        }

        private bool ChannelsAction(Room room, TextChannel textChannel) //TODO: delete empty method
        {
            var actions = new string[] {"edit", "delete"};
            
            System.Console.Write($"What do you want to do? ({string.Join("or", actions)})");
            string action;
            do
            {
                action = System.Console.ReadLine();
            } while (action == null || !actions.Contains(action));

            switch (action)
            {
                case "edit":
                    return EditTextChannel(room, textChannel);
                case "delete":
                    return DeleteTextChannel(room, textChannel);
            }
            
            return true;
        }

        private bool DeleteTextChannel(Room room, TextChannel textChannel)
        {
            if (!_textChannelService.CanManageChannels(room).Result || 
                (!_textChannelService.CanUseAdminChannels(room).Result && textChannel.IsAdminChannel))
            {
                return false;
            }

            return _textChannelService.DeleteTextChannel(textChannel, room).Result;
        }
        
        private bool EditTextChannel(Room room, TextChannel textChannel)
        {
            if (!_textChannelService.CanManageChannels(room).Result || 
                (!_textChannelService.CanUseAdminChannels(room).Result && textChannel.IsAdminChannel))
            {
                return false;
            }
            
            System.Console.Write("Enter new channel name (if you  don't want to change it - just press Enter): ");
            var name = System.Console.ReadLine();
            
            System.Console.Write("Enter new channel description (if you  don't want to change it - just press Enter): ");
            var description = System.Console.ReadLine();

            if (_textChannelService.CanUseAdminChannels(room).Result)
            {
                var actions = new string[] {"y", "n", null};
            
                System.Console.Write("Do you want to do it private? ");
                string admin;
                do
                {
                    admin = System.Console.ReadLine();
                } while (!actions.Contains(admin));

                bool? isAdmin = null;
                switch (admin)
                {
                    case "y":
                        isAdmin = true;
                        break;
                    case "n":
                        isAdmin = false;
                        break;
                }
                
                return _textChannelService.EditTextChannel(textChannel, room, name, description, isAdmin).Result;
            }

            System.Console.WriteLine("Error! Please, try again later!");
            
            return false;
        }
    }
}

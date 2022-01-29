using System.Linq;
using BLL.Abstractions.Interfaces;
using Core;

namespace PL.Console.RoomsControl
{
    public class TextChannelControl
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

            foreach (var channel in channelList)
            {
                System.Console.WriteLine($"{counter}) {channel.ChannelName}");
                counter++;
            }

            var actions = new string[] {"create"};
            
            System.Console.Write("What do you want to do? ");
            string action;
            var outAction = -1;
            do
            {
                action = System.Console.ReadLine();
                int.TryParse(action, out outAction);
            } while (action == null || (!actions.Contains(action) && outAction <= channelList.Count && outAction > 0));

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

            return ChannelsAction(channelList[outAction - 1]); //TODO: text channel logic
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
                System.Console.Write("Do you want to this channel be private (\"y\" or \"n\")? ");
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

        private bool ChannelsAction(TextChannel textChannel) //TODO: delete empty method
        {
            return true;
        }
    }
}

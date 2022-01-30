﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using PL.Console.Interfaces;

namespace PL.Console.RoomsControl
{
    public class PersonalChatControl : IPersonalChatControl
    {
        private readonly IPersonalChatService _chatService;
        private readonly IUserValidator _userValidator;
        private readonly ICurrentUser _currentUser;

        public PersonalChatControl(IPersonalChatService chatService, IUserValidator userValidator,
            ICurrentUser currentUser)
        {
            this._chatService = chatService;
            this._userValidator = userValidator;
            this._currentUser = currentUser;
        }

        public void StartChat()
        {
            System.Console.WriteLine(
                "Enter username / email that you want to invite(if many users enumerate them with \"space\")");
            var userNames = System.Console.ReadLine().Split(" ");
            var usersToInvite = new List<string>();
            var errors = new List<string>();
            foreach (var name in userNames)
            {
                var validationsResult = _userValidator.ValidateUserNameOrEmail(name);
                if (validationsResult == true)
                {
                    usersToInvite.Add(name);
                }
                else
                {
                    errors.Add(name);
                }
            }

            if (errors.Any())
            {
                System.Console.WriteLine("Such users have not been added to group chat:");
                foreach (var error in errors)
                {
                    System.Console.WriteLine(error);
                }
            }

            // System.Console.WriteLine($"{userName}\n start typing:");
            //here must be called message worker or smth like this
            //when first message is sent, then chat is initiated

            //on message sent:

            _chatService.CreatePersonalChat(userNames);
        }

        public async Task<IList<PersonalChat>> GetUserPersonalChats()
        {
            var chats = await _chatService.GetUserPersonalChats();
            var chatsNames = chats.Select(chat => chat.ChatName).ToList();

            if (!chats.Any())
            {
                System.Console.WriteLine("You have no chats");
            }
            else
            {
                System.Console.WriteLine("Your chats: ");

                for (var i = 0; i < chatsNames.Count; i++)
                {
                    System.Console.WriteLine($"\t{i + 1}) {chatsNames[i]}");
                }
            }

            return chats;
        }

        public void DoAction()
        {
            var userChats = GetUserPersonalChats().Result;
            string userInput;
            do
            {
                System.Console.Write(
                    "If you want to choose chat - type its number or if you want to start new chat enter - \"start\": ");
                userInput = System.Console.ReadLine()?.Trim();
            } while (userInput == null && userInput != "start" && !int.TryParse(userInput, out _));

            if (userInput == "start")
            {
                StartChat();
            }

            else if (int.TryParse(userInput, out var chatNumber))
            {
                EnterPersonalChat(userChats[chatNumber - 1]);
            }
        }

        private void EnterPersonalChat(PersonalChat chat)
        {
            System.Console.WriteLine($"You successfully entered chat {chat.ChatName}");
            System.Console.WriteLine("What do you want to do?");

            string userInput;
            do
            {
                System.Console.WriteLine(
                    "To leave personal chat, type \"leave\", to change name of personal chat type \"change\"," +
                    " to invite users type \"invite\"" +
                    "to see users type \"users\"");
                userInput = System.Console.ReadLine()?.Trim();
                userInput = userInput == string.Empty ? null : userInput;
            } while (userInput == null && userInput != "leave" && userInput != "change" && userInput != "users" &&
                     userInput != "invite");


            if (userInput == "leave")
            {
                _chatService.LeavePersonalChat(chat).Wait();
            }

            if (userInput == "users")
            {
                var users =  _chatService.GetUserNamesOfChat(chat).Result;
                System.Console.WriteLine(string.Join("\n", users));
            }

            if (userInput == "change")
            {
                System.Console.WriteLine("Enter new name for chat:");
                var name = System.Console.ReadLine();

                _ = _chatService.ChangeNameOfPersonalChat(chat, name).Result;
            }

            if (userInput == "invite")
            {
                System.Console.WriteLine("Enumerate users to invite with space ");
                var usersToInvite = System.Console.ReadLine()?.Split(" ");
                _chatService.AddParticipantsToPersonalChat(chat, usersToInvite).Wait();
            }
        }
    }
}
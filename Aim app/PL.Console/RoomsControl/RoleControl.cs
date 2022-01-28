using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using PL.Console.Interfaces;

namespace PL.Console.RoomsControl
{
    public class RoleControl : IRoleControl
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        
        public RoleControl(IRoleService roleService, IUserService userService)
        {
            _roleService = roleService;
            _userService = userService;
        }
        
        public async Task ViewRolesInTheRoom(Room room)
        {
            var roles = await _roleService.GetAllRolesInRoom(room);
            
            if (roles.Count == 0)
            {
                System.Console.WriteLine("There is no roles in room");
            }
            else
            {
                System.Console.WriteLine("Roles in this room: ");

                for (var i = 0; i < roles.Count; i++)
                {
                    System.Console.WriteLine($"\t{i + 1}) {roles[i].RoleName}");
                }
            }
            
            string userInput;
            do
            {
                System.Console.Write(
                    "If you want to choose role - type its number or if you want to create new one - enter \"create\" or \"my role\" - to see your role: ");
                userInput = System.Console.ReadLine()?.Trim();
            } while (userInput == null && userInput != "create" && userInput != "my role" && !int.TryParse(userInput, out var roleNumber));

            
            if (userInput == "create")
            {
                await CreateRole(room);
            }
            if (userInput == "my role")
            {
                await ViewMyRole(room);
            }
            else if (int.TryParse(userInput, out var roleNumber) && roles.Count >= roleNumber)
            {
                await ChooseRoleAction(roles[roleNumber - 1]);
            }
        }

        private async Task<bool> CreateRole(Room room)
        {
            System.Console.WriteLine("Enter role name: ");
            var roleName = System.Console.ReadLine()?.Trim();

            var response = await _roleService.CreateNewRole(room, roleName);
            System.Console.WriteLine("Role successfully created");
            
            //Todo: Add permissions
            
            return response;
        }

        private async Task<bool> ViewMyRole(Room room)
        {
            try
            {
                var myRole = await _userService.GetRoleInRoom(room);

                System.Console.WriteLine($"Your role - {myRole.RoleName}");
                System.Console.WriteLine("Permissions:");
                System.Console.WriteLine($"\t-Can Pin: {myRole.CanPin}");
                System.Console.WriteLine($"\t-Can Invite: {myRole.CanInvite}");
                System.Console.WriteLine($"\t-Can Delete Others Messages: {myRole.CanDeleteOthersMessages}");
                System.Console.WriteLine($"\t-Can Moderate Participants: {myRole.CanModerateParticipants}");
                System.Console.WriteLine($"\t-Can Manage Roles: {myRole.CanManageRoles}");
                System.Console.WriteLine($"\t-Can Manage Channels: {myRole.CanManageChannels}");
                System.Console.WriteLine($"\t-Can Manage Room: {myRole.CanManageRoom}");
                System.Console.WriteLine($"\t-Can Use Admin Channels: {myRole.CanUseAdminChannels}");
                System.Console.WriteLine($"\t-Can View Audit Log: {myRole.CanViewAuditLog}");

                return true;
            }
            catch (Exception)
            {
                System.Console.WriteLine("Error! Please, try again later!");
                return false;
            }
            
        }

        private async Task<bool> ChooseRoleAction(Room room, Role role)
        {
            string userInput;
            do{
                System.Console.WriteLine(
                "If you want to edit permissions - type \"edit\" or if you want to delete role - type \"delete\"");
                userInput = System.Console.ReadLine()?.Trim();
            } while (userInput == null && userInput != "edit" && userInput != "delete");

            switch (userInput)
            {
                case "edit":
                    
                    break;
                case "delete":
                    
                    break;
            }
            
            return true;
        }

        private async Task<bool> SetUpRole(Room room, Role role)
        {
            var permissions = new Dictionary<string, bool?>();

            permissions.Add();
            
            
            
            System.Console.WriteLine($"\t-Can Pin: ");
            System.Console.WriteLine($"\t-Can Invite: ");
            System.Console.WriteLine($"\t-Can Delete Others Messages: ");
            System.Console.WriteLine($"\t-Can Moderate Participants: ");
            System.Console.WriteLine($"\t-Can Manage Roles: ");
            System.Console.WriteLine($"\t-Can Manage Channels: ");
            System.Console.WriteLine($"\t-Can Manage Room: ");
            System.Console.WriteLine($"\t-Can Use Admin Channels: ");
            System.Console.WriteLine($"\t-Can View Audit Log: ");

            return await _roleService.SetUpRole();
        }

        private async Task<bool> DeleteRole(Room room, Role role)
        {
            
        }
    }
}

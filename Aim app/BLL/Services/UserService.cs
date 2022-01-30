using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userGenericRepository;
        private readonly IGenericRepository<Room> _roomGenericRepository;
        private readonly IGenericRepository<Role> _roleGenericRepository;
        private readonly ICurrentUser _currentUser;

        public UserService(IGenericRepository<User> userGenericRepository,
            IGenericRepository<Room> roomGenericRepository, IGenericRepository<Role> roleGenericRepository,
            ICurrentUser currentUser)
        {
            _userGenericRepository = userGenericRepository;
            _roomGenericRepository = roomGenericRepository;
            _roleGenericRepository = roleGenericRepository;
            _currentUser = currentUser;
        }

        public async Task<bool> LeaveRoom(Room room)
        {
            if (room == null)
            {
                return false;
            }

            room.Participants.Remove(
                room.Participants.FirstOrDefault(participant => participant.UserId == _currentUser.User.Id));
            await _roomGenericRepository.UpdateAsync(room);
            return true;
        }

        public async Task<bool> SwitchNotifications(Room room, bool stateOnOrOff)
        {
            if (room == null)
            {
                return false;
            }

            room.Participants.FirstOrDefault(participant => participant.UserId == _currentUser.User.Id)!
                    .Notifications =
                stateOnOrOff;
            await _roomGenericRepository.UpdateAsync(room);
            return true;
        }

        public bool IsUserVerified(User user)
        {
            return user.IsVerified;
        }

        public async Task<List<Room>> GetUserRooms()
        {
            var currentUser = _currentUser.User;

            var foundRooms = await _roomGenericRepository
                .FindByConditionAsync(room =>
                    room.Participants.Any(participant => participant.UserId == currentUser.Id));

            return foundRooms.ToList();
        }

        public async Task ChangeUserNames(string firstName = null, string lastName = null)
        {
            var user = _currentUser.User;

            if (firstName != null)
            {
                user.FirstName = firstName;
            }

            if (lastName != null)
            {
                user.LastName = lastName;
            }

            await _userGenericRepository.UpdateAsync(user);
        }
        
        public async Task<Role> GetRoleInRoom(Room room)
        {
            var currentUser = _currentUser.User;
            var roleId = room.Participants.FirstOrDefault(participantInfo => participantInfo.UserId == currentUser.Id)?.RoleId;
            var roles = await _roleGenericRepository.FindByConditionAsync(role => role.Id == roleId);
            
            return roles.FirstOrDefault();
        }

        public async Task<User> GetUserByUserNameOrEmail(string userName)
        {
            var users = await _userGenericRepository.FindByConditionAsync(user =>
                user.UserName == userName || user.Email == userName);
            
            return users.FirstOrDefault();
        }
    }
}

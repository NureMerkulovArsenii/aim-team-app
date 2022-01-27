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
        private readonly ICurrentUser _currentUser;

        public UserService(IGenericRepository<User> userGenericRepository,IGenericRepository<Room> roomGenericRepository, ICurrentUser currentUser)
        {
            _userGenericRepository = userGenericRepository;
            _roomGenericRepository = roomGenericRepository;
            _currentUser = currentUser;
        }

        public async Task<bool> LeaveRoom(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                return false;
            }

            var room = _roomGenericRepository
                .FindByConditionAsync(room => room.Id == roomId)
                .Result
                .FirstOrDefault();
            
            if (room == null)
            {
                return false;
            }

            room.Participants.Remove(
                room.Participants.FirstOrDefault(participant => participant.User.Id == _currentUser.User.Id));
            await _roomGenericRepository.UpdateAsync(room);
            return true;
        }

        public async Task<bool> SwitchNotifications(string roomId, bool stateOnOrOff)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                return false;
            }

            var room = _roomGenericRepository
                .FindByConditionAsync(room => room.Id == roomId)
                .Result
                .FirstOrDefault();

            if (room == null)
            {
                return false;
            }

            room.Participants.FirstOrDefault(participant => participant.User.Id == _currentUser.User.Id)!.Notifications =
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
                    room.Participants.Any(participant => participant.User.Id == currentUser.Id));

            return foundRooms.ToList();
        }
    }
}

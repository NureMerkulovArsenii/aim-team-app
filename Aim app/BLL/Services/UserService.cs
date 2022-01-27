using System;
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
            
            room.Participants.Remove(_currentUser.User);
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

            room.Participants[_currentUser.User].Notifications = stateOnOrOff;
            await _roomGenericRepository.UpdateAsync(room);
            return true;
        }

        public bool IsUserVerified(User user)
        {
            return user.IsVerified;
        }
    }
}

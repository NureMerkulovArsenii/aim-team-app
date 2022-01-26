using System;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _genericRepository;

        public UserService(IGenericRepository<User> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public Task LeaveRoom(string roomId)
        {
            throw new NotImplementedException();
        }

        public Task SwitchNotifications(string roomId, bool stateOnOrOff)
        {
            throw new NotImplementedException();
        }
    }
}

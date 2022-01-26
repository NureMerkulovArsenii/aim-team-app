using System;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class RoomService : IRoomService
    {
        private readonly IGenericRepository<Room> _genericRepository;

        public RoomService(IGenericRepository<Room> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        
        public Task<Users> GetParticipantsOfRoom(string roomId)
        {
            throw new NotImplementedException();
        }
    }
}

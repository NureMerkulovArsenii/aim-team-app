using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class RoomService : IRoomService
    {
        private readonly IGenericRepository<Room> _roomGenericRepository;

        public RoomService(IGenericRepository<Room> genericRepository)
        {
            _roomGenericRepository = genericRepository;
        }
        
        public async Task<List<User>> GetParticipantsOfRoom(string roomId)
        {
            var rooms = await _roomGenericRepository.FindByConditionAsync(room => room.Id == roomId);
            var room = rooms.FirstOrDefault();

            return room.Participants.Keys.ToList();
        }
    }
}

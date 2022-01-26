﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Core;

namespace BLL.Abstractions.Interfaces
{
    public interface IRoomService
    {
        Task<Users> GetParticipantsOfRoom(string roomId);
    }
}
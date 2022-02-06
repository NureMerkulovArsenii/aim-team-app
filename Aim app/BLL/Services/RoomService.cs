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
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(ICurrentUser currentUser, IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;

            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateRoom(string name, string description) //TODO: Photo implementation
        {
            var user = _currentUser.User;

            var adminRole = new Role("Admin")
            {
                CanInvite = true,
                CanDeleteOthersMessages = true,
                CanManageChannels = true,
                CanManageRoles = true,
                CanManageRoom = true,
                CanModerateParticipants = true,
                CanPin = true,
                CanUseAdminChannels = true,
                CanViewAuditLog = true
            };

            var baseRole = new Role("User");

            var adminParticipantInfo = new ParticipantInfo()
            {
                Notifications = true, User = user, Role = adminRole
            };

            var room = new Room()
            {
                RoomName = name,
                RoomDescription = description,
                Participants = new List<ParticipantInfo> {adminParticipantInfo},
                RoomRoles = new List<Role>() {adminRole, baseRole},
                TextChannels = new List<TextChannel>()
            };

            try
            {
                await _unitOfWork.CreateTransactionAsync();

                await _unitOfWork.RoomRepository.UpdateAsync(room);

                await _unitOfWork.RoleRepository.CreateAsync(baseRole);
                
                await _unitOfWork.RoleRepository.CreateAsync(adminRole);

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
            }

            return room.Id;
        }

        public async Task<bool> DeleteRoom(Room room)
        {
            var user = _currentUser.User;

            if (IsUserAdmin(room, user))
            {
                await _unitOfWork.RoomRepository.DeleteAsync(room);

                return true;
            }

            return false;
        }

        public async Task<bool> ChangeRoomSettings(Room room, string name, string description) //TODO: photo implementation
        {
            var user = _currentUser.User;

            if (IsUserAdmin(room, user))
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    room.RoomName = name;
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    room.RoomDescription = description;
                }
                
                await _unitOfWork.RoomRepository.UpdateAsync(room);

                return true;
            }

            return false;
        }

        private bool IsUserAdmin(Room room, User user)
        {
            var isUserAdmin = room.Participants
                .Where(participant => participant.Role.CanManageRoom && participant.User.Id == user.Id);

            return isUserAdmin.Any();
        }

        public async Task<List<User>> GetParticipantsOfRoom(int roomId)
        {
            var room = _unitOfWork.RoomRepository
                .FindByConditionAsync(room => room.Id == roomId)
                .Result
                .FirstOrDefault();

            var users = room?.Participants.Select(participant => participant.User).ToList();

            return users;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BLL.Services
{
    public class UrlInvitationService : IUrlInvitationService
    {
        private readonly IGenericRepository<Urls> _genericRepositoryUrls;
        private readonly IGenericRepository<Room> _genericRepositoryRooms;
        private readonly ICurrentUser _currentUser;
        private readonly IMailWorker _mailWorker;
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;

        public UrlInvitationService(IGenericRepository<Urls> genericRepositoryUrls,
            IGenericRepository<Room> genericRepositoryRooms, ICurrentUser currentUser,
            IMailWorker mailWorker, IUserService userService, IOptions<AppSettings> appSettings)
        {
            this._genericRepositoryUrls = genericRepositoryUrls;
            this._genericRepositoryRooms = genericRepositoryRooms;
            this._currentUser = currentUser;
            this._mailWorker = mailWorker;
            this._userService = userService;
            this._appSettings  = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public async Task InviteUsersByEmailWithUrlAsync(Room room, List<string> users)
        {
            var currentTime = DateTime.Now;
            var expirationTime = currentTime.AddMinutes(15).ToString(CultureInfo.InvariantCulture);

            var result = new List<string>();

            var url = new Urls {RoomId = room.Id, Url = _appSettings.Domain + Guid.NewGuid().ToString().Substring(0, 7),};


            foreach (var user in users)
            {
                var userId = _userService.GetUserByUserNameOrEmail(user).Result;
                if (room.Participants.Any(info => info.UserId == userId.Id))
                {
                    return;
                }

                result.Add(userId.Id);
                await _mailWorker.SendInvitationEmailAsync(room, url.Url, userId.Email); //ToDO bool
            }

            url.UserId = result;
            url.ExpirationTime = expirationTime;

            await _genericRepositoryUrls.CreateAsync(url);
        }

        public string InviteUsersByUrl(Room room)
        {
            var currentTime = DateTime.Now;
            var expirationTime = currentTime.AddHours(5).ToString(CultureInfo.InvariantCulture);
            var url = new Urls
            {
                Url = _appSettings.Domain + Guid.NewGuid().ToString().Substring(0, 6),
                ExpirationTime = expirationTime,
                UserId = null,
                RoomId = room.Id
            };

            _genericRepositoryUrls.CreateAsync(url);
            
            return url.Url;
        }

        public async Task<bool> JoinByUrl(string url)
        {
            var urlFromDb = await _genericRepositoryUrls.FindByConditionAsync(urls => urls.Url == url);

            var urlsEnumerable = urlFromDb.ToList();
            if (urlsEnumerable.Any())
            {
                var responseUrl = urlsEnumerable.FirstOrDefault();
                var now = DateTime.Now;
                var expirationTime = DateTime.Parse(responseUrl.ExpirationTime, CultureInfo.InvariantCulture);

                if (responseUrl.UserId == null ||
                    responseUrl.UserId.Contains(_currentUser.User.Id) && expirationTime >= now)
                {
                    var rooms =
                        await _genericRepositoryRooms.FindByConditionAsync(room => room.Id == responseUrl.RoomId);

                    var room = rooms.FirstOrDefault();
                    var participantInfo = new ParticipantInfo()
                    {
                        Notifications = true, UserId = _currentUser.User.Id, RoleId = room.BaseRoleId
                    };
                    
                    if (room.Participants.All(info => info.UserId != participantInfo.UserId))
                    {
                        room.Participants.Add(participantInfo);
                    }

                    await _genericRepositoryRooms.UpdateAsync(room);
                    
                    return true;
                }
            }

            return false;
        }
    }
}

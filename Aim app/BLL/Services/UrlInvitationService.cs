using System;
using System.Globalization;
using BLL.Abstractions.Interfaces;
using Core;
using DAL.Abstractions.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using BLL.Helpers;

namespace BLL.Services
{
    public class UrlInvitationService : IUrlInvitationService
    {
        private string _domen = "aim-app.net";
        private readonly IGenericRepository<Urls> _genericRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IMailWorker _mailWorker;

        public UrlInvitationService(IGenericRepository<Urls> genericRepository, ICurrentUser currentUser,
            IMailWorker mailWorker)
        {
            this._genericRepository = genericRepository;
            this._currentUser = currentUser;
            this._mailWorker = mailWorker;
        }

        public void InviteUserByUrl(Room roomId, User user)
        {
            var currentTime = DateTime.Now;
            var expirationTime = currentTime.AddMinutes(5).ToString("hh.mm.ss", CultureInfo.InvariantCulture);
            var url = new Urls {Url = _domen + roomId + user.Id, UserId = user.Id, ExpirationTime = expirationTime};

            _genericRepository.CreateAsync(url);
        }

        public string InviteUsersByUrl(Room roomId)
        {
            var currentTime = DateTime.Now;
            var expirationTime =
                currentTime.AddMinutes(5).ToString("yyyy.mm.dd.hh.mm.ss", CultureInfo.InvariantCulture);
            var url = new Urls
            {
                Url = _domen + roomId + Guid.NewGuid().ToString().Substring(0, 6),
                ExpirationTime = expirationTime,
                UserId = string.Empty
            };

            _genericRepository.CreateAsync(url);
            return url.Url;
        }

        public async Task<bool> JoinByUrl(string url)
        {
            var urlFromDb = await _genericRepository.FindByConditionAsync(urls => urls.Url == url);

            var urlsEnumerable = urlFromDb.ToList();
            if (urlsEnumerable.Any())
            {
                var expectedUserId = urlsEnumerable.First().UserId;
                if (_currentUser.User.Id == expectedUserId)
                {
                    //ToDO: put user into room
                    return true;
                }
            }

            return false;
        }
    }
}

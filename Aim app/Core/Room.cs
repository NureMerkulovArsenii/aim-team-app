using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Core
{
    public class Room : BaseEntity
    {
        // private string _filePath;

        public string RoomName { get; set; }

        public string RoomDescription { get; set; }

        public IDictionary<User, ParticipantInfo> Participants { get; set; }

        [JsonIgnore]
        public ReadOnlyCollection<byte> Photo { get; set; }

        public string PhotoSource
        {
            get
            {
                if (this.Photo != null)
                {
                    return Convert.ToBase64String(this.Photo.ToArray());
                }

                return string.Empty;
            }
            set { this.Photo = Array.AsReadOnly(Convert.FromBase64String(value)); }
        }
    }
}

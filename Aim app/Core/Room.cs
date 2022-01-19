using System.Drawing;

namespace Core
{
    public class Room
    {
        private string _filePath;
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public Bitmap Photo { get; set; }

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                Photo = new Bitmap(_filePath);
            }
        }
    }
}
namespace Core
{
    public class Urls : BaseEntity
    {
        public string Url { get; set; }
        public string ExpirationTime { get; set; }
        public string UserId { get; set; }
        public bool IsUsed { get; set; }
    }
}

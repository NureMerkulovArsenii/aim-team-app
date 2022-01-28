namespace Core
{
    public class TextChat : BaseEntity
    {
        public string ChatName { get; set; }
        
        public string ChatDescription { get; set; }

        public bool IsAdmin { get; set; }
    }
}

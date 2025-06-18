namespace WebApp.Models
{
    public class Comments
    {
        public string UserName { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

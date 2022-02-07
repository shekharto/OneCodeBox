namespace EventDrivenPostService.Model
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }
        // public User User { get; set; }
    }
}

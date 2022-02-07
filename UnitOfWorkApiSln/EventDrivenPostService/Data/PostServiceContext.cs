using Microsoft.EntityFrameworkCore;

namespace EventDrivenPostService.Data
{
    public class PostServiceContext : DbContext
    {
        public PostServiceContext(DbContextOptions<PostServiceContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Model.Post> Posts { get; set; }
        public DbSet<Model.User> User { get; set; }

    }
}

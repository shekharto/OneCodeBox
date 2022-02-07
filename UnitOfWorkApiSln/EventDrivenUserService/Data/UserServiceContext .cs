using Microsoft.EntityFrameworkCore;

namespace EventDrivenUserService.Data
{
    public class UserServiceContext : DbContext
    {
        public UserServiceContext(DbContextOptions<UserServiceContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Model.User> Users { get; set; }
    }
}

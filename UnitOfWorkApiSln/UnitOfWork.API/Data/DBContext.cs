using Microsoft.EntityFrameworkCore;
using UnitOfWork.API.Model;

namespace UnitOfWork.API.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }

        public virtual DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder   modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }



    }
}

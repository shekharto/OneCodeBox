using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace UserManagement.JWT.Context
{
    public class SqliteDataContext : DataContext
    {

        //  public SqliteDataContext(IConfiguration configuration) : base(configuration) { }

        public SqliteDataContext(IConfiguration configuration) : base(null) { }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite server
            options.UseSqlite(_configuration.GetConnectionString("WebApiDatabase"));
        }
    }
}

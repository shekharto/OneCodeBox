using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Entities;
using UserManagement.JWT.Interface;
using UserManagement.JWT.Models.Config;

namespace UserManagement.JWT.Context
{
    public class DataContext : DbContext, IDataContext
    {
        protected readonly IConfiguration _configuration;
        public DbSet<JwtUser> JwtUser { get; set; }
        public DbSet<JwtRefreshToken> JwtRefreshToken { get; set; }
        public DbSet<JwtRole> JwtRole { get; set; }
        public DbSet<JwtUserRole>  JwtUserRole { get; set; }
        public DbSet<JwtTask> JwtTask { get; set; }
        public DbSet<JwtRoleTask> JwtRoleTask { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
             base.OnModelCreating(builder);
            builder.ApplyConfiguration(new JwtUserConfig());
            builder.ApplyConfiguration(new JwtRefreshTokenConfig());
            builder.ApplyConfiguration(new JwtRoleConfig());
            builder.ApplyConfiguration(new JwtUserRoleConfig());
            builder.ApplyConfiguration(new JwtTaskConfig());
            builder.ApplyConfiguration(new JwtRoleTaskConfig());

        }
         
    }
}

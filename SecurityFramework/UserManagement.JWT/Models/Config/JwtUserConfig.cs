using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Entities;

namespace UserManagement.JWT.Models.Config
{
   
    public class JwtUserConfig : IEntityTypeConfiguration<JwtUser>
    { 
        public void Configure(EntityTypeBuilder<JwtUser> builder)
        {
            builder.ToTable("JwtUser");
            builder.HasKey(m => m.Id);
 
             builder.HasMany(r => r.JwtUserRoles).WithOne(r => r.JwtUser).HasForeignKey(f => f.UserID);
        } 
    }
}

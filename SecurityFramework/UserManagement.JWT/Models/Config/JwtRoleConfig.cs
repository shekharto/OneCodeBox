using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Entities;

namespace UserManagement.JWT.Models.Config
{
   
    public class JwtRoleConfig : IEntityTypeConfiguration<JwtRole>
    {
        public void Configure(EntityTypeBuilder<JwtRole> builder)
        {
            builder.ToTable("JwtRole");
            builder.HasKey(m => m.Id);

          builder.HasMany(r => r.JwtRoleTasks).WithOne(r => r.JwtRole).HasForeignKey(f => f.RoleId);

        }
    }
}

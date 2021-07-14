using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Entities;

namespace UserManagement.JWT.Models.Config
{
 

    public class JwtUserRoleConfig : IEntityTypeConfiguration<JwtUserRole>
    {
        public void Configure(EntityTypeBuilder<JwtUserRole> builder)
        {
            builder.ToTable("JwtUserRole");
            builder.HasKey(m => m.Id);

          //  builder.HasOne(r => r.JwtUser).WithMany(t => t.JwtUserRoles).HasForeignKey(fk => fk.UserID);

           builder.HasOne(p => p.JwtRole).WithMany(c => c.JwtUserRoles).HasForeignKey(fk => fk.RoleID);
        }
    }

}

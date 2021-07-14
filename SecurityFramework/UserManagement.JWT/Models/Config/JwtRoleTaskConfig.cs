using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Entities;

namespace UserManagement.JWT.Models.Config
{
 
    public class JwtRoleTaskConfig : IEntityTypeConfiguration<JwtRoleTask>
    {
        public void Configure(EntityTypeBuilder<JwtRoleTask> builder)
        {
            builder.ToTable("JwtRoleTask");
            builder.HasKey(m => m.Id);


            builder.HasOne(p => p.JwtTask).WithMany(c => c.JwtRoleTasks).HasForeignKey(fk => fk.RoleId);
        }
    }

}

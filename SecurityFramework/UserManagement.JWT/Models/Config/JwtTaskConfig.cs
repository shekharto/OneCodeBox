using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Entities;

namespace UserManagement.JWT.Models.Config
{
 
    public class JwtTaskConfig : IEntityTypeConfiguration<JwtTask>
    {
        public void Configure(EntityTypeBuilder<JwtTask> builder)
        {
            builder.ToTable("JwtTask");
            builder.HasKey(m => m.Id);

        }
    }

}

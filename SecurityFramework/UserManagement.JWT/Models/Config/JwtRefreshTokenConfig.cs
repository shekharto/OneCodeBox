using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Entities;

namespace UserManagement.JWT.Models.Config
{
   
    public class JwtRefreshTokenConfig : IEntityTypeConfiguration<JwtRefreshToken>
    {
        public void Configure(EntityTypeBuilder<JwtRefreshToken> builder)
        {
            builder.ToTable("JwtRefreshToken");
            builder.HasKey(m => m.Id);
            builder.HasOne(p => p.jwtUsers).WithMany(c => c.refreshTokens).HasForeignKey(fk => fk.UserId);
            
        }

    }
}

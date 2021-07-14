using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Entities;

namespace UserManagement.JWT.Models.Config
{
    //public class JwtLoginConfig : IEntityTypeConfiguration<JwtLogin>
    //{
       

    //    public   void Configure(EntityTypeBuilder<JwtLogin> builder)
    //    {
    //        builder.ToTable("JwtLogin");
    //        builder.HasKey(m => m.Id);
    //      //  builder.Ignore(m => m.refreshToken);
    //    }

    //}

    //public virtual void Configure(EntityTypeBuilder<JwtLogin> builder)
    //{
    //    builder.ToTable("JwtLogin");
    //    builder.HasKey(m => m.RoleId);
 
    //    base.Configure(builder);
    //}
}
 

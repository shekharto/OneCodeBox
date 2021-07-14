using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Entities;

namespace UserManagement.JWT.Interface
{
    public interface IDataContext
    {
        public DbSet<JwtUser> JwtUser { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Entities;
using UserManagement.JWT.Models.Users;

namespace UserManagement.JWT.Interface
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(JwtUser user, string roles, string taskids);
        public int? ValidateJwtToken(string token);
        public JwtRefreshToken GenerateRefreshToken(string ipAddress);
    }
}

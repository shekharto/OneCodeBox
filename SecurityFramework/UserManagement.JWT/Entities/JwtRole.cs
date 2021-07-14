using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.JWT.Entities
{
    public class JwtRole
    {
        [Key]
        public int Id { get; set; }
        public string RoleName { get; set; }

        public List<JwtUserRole>  JwtUserRoles { get; set; }
        public List<JwtRoleTask> JwtRoleTasks { get; set; }
    }
}

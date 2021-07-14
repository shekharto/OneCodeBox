using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.JWT.Entities
{
    public class JwtRoleTask
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int TaskId { get; set; }

        public JwtRole JwtRole { get; set; }
        public JwtTask  JwtTask { get; set; }
    }
}

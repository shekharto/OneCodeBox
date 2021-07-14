using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.JWT.Entities
{
    public class JwtUserRole
    {
         [Key]
        public int Id { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }


        public JwtRole  JwtRole { get; set; }
        public JwtUser JwtUser { get; set; }   
    }
}

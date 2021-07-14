using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UserManagement.JWT.Models.Users;

namespace UserManagement.JWT.Entities
{
    public class JwtUser
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public DateTime UpdatedDtm { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
         
        [JsonIgnore]
        public List<JwtRefreshToken> refreshTokens { get; set; }

        public List<JwtUserRole>  JwtUserRoles { get; set; }
    }




    //public class JwtLogin
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Username { get; set; }
    //    public DateTime UpdatedDtm { get; set; }
    //    [JsonIgnore]
    //    public string PasswordHash { get; set; }


    //    [JsonIgnore]
    //    public List<JwtRefreshToken> refreshToken { get; set; }

    //}



    //public class JwtUser
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Username { get; set; }
    //    [JsonIgnore]
    //    public string PasswordHash { get; set; }

    //}
}

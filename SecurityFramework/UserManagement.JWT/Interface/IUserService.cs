using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Entities;
using UserManagement.JWT.Models.Users;

namespace UserManagement.JWT.Interface
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        IEnumerable<JwtUser> GetAll();
      //  IEnumerable<JwtLogin> GetAllLogin();
        List<JwtUser> JwtGetAll();
        JwtUser GetById(int id);
        void Register(RegisterRequest model);
        void Update(int id, UpdateRequest model);
        void Delete(int id);
    }
}

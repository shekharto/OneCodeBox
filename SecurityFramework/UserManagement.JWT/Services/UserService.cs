using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Context;
using UserManagement.JWT.Entities;
using UserManagement.JWT.Helpers;
using UserManagement.JWT.Interface;
using UserManagement.JWT.Models.Users;
using BCryptNet = BCrypt.Net.BCrypt;

namespace UserManagement.JWT.Services
{
    public class UserService : IUserService
    {
        private DataContext _context;
        private IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private string userRoles;
        private string roleTasks;

        public UserService(DataContext context, IJwtUtils jwtUtils, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
            _appSettings = appSettings.Value;
 
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest authenticateRequest, string ipAddress)
        {
            var user = _context.JwtUser.SingleOrDefault(x => x.Username == authenticateRequest.Username);

            //validate
            if (user == null || !BCryptNet.Verify(authenticateRequest.Password, user.PasswordHash))
                throw new AppException("Username or password is incorrect");

            //authentiate successful
            //   var response = _mapper.Map<AuthenticateResponse>(user);

            //get the role and task for loged in user
            GetUserRolesAndTasks(user.Id);
           
            var JwtToken = _jwtUtils.GenerateJwtToken(user, userRoles, roleTasks);
            var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            user.refreshTokens = new List<JwtRefreshToken>();
            user.refreshTokens.Add(refreshToken);

            // remove old refresh token from user
            removeOldRefreshTokens(user);
            _context.Update(user);
            _context.SaveChanges();

            return new AuthenticateResponse(user, JwtToken, refreshToken.Token);             
        }

        public void Register(RegisterRequest registerRequest)
        {
            //validate
            if (_context.JwtUser.Any(x => x.Username == registerRequest.Username))
                throw new AppException("Username '" + registerRequest.Username + "' is already taken");

            //map model
            var user = _mapper.Map<JwtUser>(registerRequest);

            //has password
            user.PasswordHash = BCryptNet.HashPassword(registerRequest.Password);
            user.UpdatedDtm = DateTime.Now;
            // save user
            _context.JwtUser.Add(user);
            _context.SaveChanges();
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var user = getUserByRefreshToken(token);
            var refreshToken = user.refreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                revokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                _context.Update(user);
                _context.SaveChanges();
            }

            if (!refreshToken.IsActive)
                throw new AppException("Invalid Token");

            // replace old refresh token with a new one (called as rotate token)
            var newRefreshToken = rotateRefreshToken(refreshToken, ipAddress);
            user.refreshTokens.Add(newRefreshToken);

            // remove old refresh tokens from user
            removeOldRefreshTokens(user);

            //save to db
            _context.Update(user);
            _context.SaveChanges();

            //generate new jwt token using refresh token (as above)
            GetUserRolesAndTasks(user.Id);
            var jwtToken = _jwtUtils.GenerateJwtToken(user, userRoles, roleTasks);
            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);

        }

        private JwtRefreshToken rotateRefreshToken(JwtRefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            revokeRefreshToken(refreshToken, ipAddress, "Replace by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        public IEnumerable<JwtUser> GetAll()
        {
            return _context.JwtUser;
        }

        //public IEnumerable<JwtLogin> GetAllLogin()
        //{
        //    return _context.JwtLogins;
        //}

        public List<JwtUser> JwtGetAll()
        {
            return _context.JwtUser.ToList();
        }



        public JwtUser GetById(int id)
        {
            return getUser(id);
        } 

        public void Update(int id, UpdateRequest updateRequest)
        {
            var user = getUser(id);

            // validate
            if (updateRequest.Username != user.Username && _context.JwtUser.Any(x => x.Username == updateRequest.Username))
                throw new AppException("Username '" + updateRequest.Username + "' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(updateRequest.Password))
                user.PasswordHash = BCryptNet.HashPassword(updateRequest.Password);

            // copy model to user and save
            _mapper.Map(updateRequest, user);
            _context.JwtUser.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = getUser(id);
            _context.JwtUser.Remove(user);
            _context.SaveChanges();
        }

        // helper methods

        private void GetUserRolesAndTasks(int id)
        {
            List<int> roleIds = _context.Set<JwtUserRole>().Where(u => u.UserID == id).Select(role => role.RoleID).ToList();
            List<JwtRole> roles = _context.JwtRole.Where(r => roleIds.Contains(r.Id)).ToList();
  
            foreach (JwtRole role in roles)
            {
                List<int> taskIds = _context.Set<JwtRoleTask>().Where(u => u.RoleId == role.Id).Select(task => task.TaskId).ToList();
                taskIds.ForEach(t =>
                {
                    roleTasks = roleTasks + t.ToString() + ", ";
                });
                userRoles = userRoles + role.RoleName + ", ";
            }

            userRoles = userRoles.Substring(0, userRoles.Length-2);
            roleTasks = roleTasks.Substring(0, roleTasks.Length - 2);
             
        }

        private void revokeDescendantRefreshTokens(JwtRefreshToken refreshToken, JwtUser user, string ipAddress, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = user.refreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken.IsActive)
                    revokeRefreshToken(childToken, ipAddress, reason);
                else
                    revokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
            }
        }

        private void revokeRefreshToken(JwtRefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }

        private JwtUser getUser(int id)
        {
            var user = _context.JwtUser.Find(id);
            if (user == null) throw new KeyNotFoundException("user not found");
            return user;
        }

        private JwtUser getUserByRefreshToken(string token)
        {

            // var user = _context.JwtUser.SingleOrDefault(u => u.refreshTokens.Any(t => t.Token == token)); 

            JwtUser user = null;
            var jwtRefreshToken = _context.JwtRefreshToken.Where(t => t.Token == token).FirstOrDefault();    
            if (jwtRefreshToken != null)
            {
                  user = _context.JwtUser.Where(t => t.Id == jwtRefreshToken.UserId).FirstOrDefault();
            }
                 
            //  var user1 = _context.Set<JwtUser>().Include(p => p.refreshTokens.Where(t => t.Token == token));

            if (user == null)
            {
                throw new AppException("Invalid   token");
            }


            return user;
        }

        private void removeOldRefreshTokens(JwtUser user)
        {
            // remove old inactive refresh token from user based on ttl in app setting.

            //***************************************done some changes for testing
            user.refreshTokens.RemoveAll(x =>
                !x.IsActive && 
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow.AddDays(3));            
        }

    }
}

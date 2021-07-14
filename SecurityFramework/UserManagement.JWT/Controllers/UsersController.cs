using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Authorization;
using UserManagement.JWT.Helpers;
using UserManagement.JWT.Interface;
using UserManagement.JWT.Models.Users;

namespace UserManagement.JWT.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
           IUserService userService,
           IMapper mapper,
           IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest authenticateRequest)
        {
            var response = _userService.Authenticate(authenticateRequest, ipAddress());
            setTokenCookies(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest registerRequest)
        {
            _userService.Register(registerRequest);
            return Ok(new { message = "Registration successful" });
        }

        [AllowAnonymous]
        [HttpPost("refreshToken")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _userService.RefreshToken(refreshToken, ipAddress());
            setTokenCookies(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("revokeToken")]
        public IActionResult RevokeToken(RevokeTokenRequest revokeTokenRequest)
        {
            // accept refresh token in request body or cookie...
            var token = revokeTokenRequest.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            _userService.RefreshToken(token, ipAddress());
            return Ok(new { message = "Token revoked" });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }


        //[AllowAnonymous]
        //[HttpGet("ids")]
        //public IActionResult GetAllLogin()
        //{
        //    var users = _userService.GetAllLogin();
        //    return Ok(users);
        //}

        [AllowAnonymous]
        [HttpGet]
        public IActionResult JwtGetAll()
        {
            var users = _userService.JwtGetAll().ToList();
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            return Ok(user);
        }

        [HttpGet("{id}/refreshtokens")]
        public IActionResult GetRefreshTokens(int id)
        {
            var user = _userService.GetById(id);
            return Ok(user.refreshTokens);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateRequest model)
        {
            _userService.Update(id, model);
            return Ok(new { message = "JwtUser updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok(new { message = "JwtUser deleted successfully" });
        }


        // helper methods

        private void setTokenCookies(string token)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            // get source ip address for the current request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forward-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

    }




}

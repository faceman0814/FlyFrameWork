using FlyFramework.Application.UserService.Dtos;
using FlyFramework.Common.Attributes;
using FlyFramework.Core.UserService;
using FlyFramework.Core.UserService.DomainService;
using FlyFramework.WebHost.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using System.Text;

using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace FlyFramework.WebHost.Controllers
{
    [ApiController]
    [DisabledUnitOfWork(true)]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginController(SignInManager<User> signInManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<string> LoginIn(LoginDto input)
        {
            //用户名和密码校验
            var result = await _signInManager.PasswordSignInAsync(input.UserName, input.Password, false, false);
            if (result.Succeeded)
            {
                //定义JWT的Playload部分
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name,input.UserName)
                };
                //生成token
                var jwtBearer = _configuration.GetSection("Authentication").GetSection("JwtBearer");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtBearer.GetValue<string>("SecurityKey")));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var securityToken = new JwtSecurityToken(
                    issuer: jwtBearer.GetValue<string>("Issuer"),
                    audience: jwtBearer.GetValue<string>("Audience"),
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                    );
                var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
                Response.Cookies.Append(
                  "access-token",
                  token,
                  new CookieOptions()
                  {
                      Expires = DateTimeOffset.UtcNow.AddMinutes(
                          30
                      )
                  }
                );
                return token;
            }
            else
            {
                return "登录失败";
            }

        }
    }
}

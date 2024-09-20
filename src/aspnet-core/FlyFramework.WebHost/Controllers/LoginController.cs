using FlyFramework.Application.UserService.Dtos;
using FlyFramework.Common.Attributes;
using FlyFramework.Common.Helpers.JWTTokens;
using FlyFramework.Common.Helpers.Redis;
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
        private readonly ICacheManager _cacheManager;
        private readonly IJWTTokenManager _jWTTokenManager;

        public LoginController(SignInManager<User> signInManager, ICacheManager cacheManager, IJWTTokenManager jWTTokenManager)
        {
            _signInManager = signInManager;
            _cacheManager = cacheManager;
            _jWTTokenManager = jWTTokenManager;
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
                var token = _jWTTokenManager.GenerateToken(claims.ToList());
                await _cacheManager.SetCache(token, input.UserName);
                return token;
            }
            else
            {
                return "登录失败";
            }
        }
    }
}

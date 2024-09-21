using FlyFramework.Common.Attributes;
using FlyFramework.Common.Utilities.JWTTokens;
using FlyFramework.Common.Utilities.Redis;
using FlyFramework.Core.UserService;
using FlyFramework.WebHost.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

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
                    new Claim(ClaimTypes.Name,input.UserName),
                };
                var token = _jWTTokenManager.GenerateToken(claims.ToList());
                await _cacheManager.SetCacheAsync(input.UserName, token);
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

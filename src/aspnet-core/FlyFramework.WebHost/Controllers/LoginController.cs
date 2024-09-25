using FlyFramework.Common.Attributes;
using FlyFramework.Common.Utilities.JWTTokens;
using FlyFramework.Common.Utilities.Redis;
using FlyFramework.Core;
using FlyFramework.Core.UserService;
using FlyFramework.Core.UserService.DomainService;
using FlyFramework.WebHost.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

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
        private readonly IUserManager _userManager;
        private readonly IStringLocalizer _sharedLocalizer;
        public LoginController(SignInManager<User> signInManager,
            ICacheManager cacheManager,
            IJWTTokenManager jWTTokenManager,
            IStringLocalizerFactory factory,
            IUserManager userManager)
        {
            _signInManager = signInManager;
            _cacheManager = cacheManager;
            _jWTTokenManager = jWTTokenManager;
            _userManager = userManager;
            _sharedLocalizer = factory.Create("FlyFramework", typeof(Program).Assembly.GetName().Name);
        }
        [HttpGet]
        public string GetString()
        {
            Console.WriteLine("资源地址：{0}", _sharedLocalizer["Name"].SearchedLocation);
            var content = _sharedLocalizer.GetString("Name").Value;
            return content;
        }
        [HttpPost]
        public async Task LoginIn(LoginDto input)
        {
            //用户名和密码校验
            var result = await _signInManager.PasswordSignInAsync(input.UserName, input.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(input.UserName);
                //定义JWT的Playload部分
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name,input.UserName),
                    new Claim(ClaimTypes.NameIdentifier,user.Id),
                };
                var token = _jWTTokenManager.GenerateToken(claims.ToList());
                await _cacheManager.SetCacheAsync(input.UserName, token);
                Response.Cookies.Append("access-token", token, new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                }
                );
            }
        }
    }
}

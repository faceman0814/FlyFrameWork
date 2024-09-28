using FlyFramework.Attributes;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.Models;
using FlyFramework.UserService;
using FlyFramework.UserService.DomainService;
using FlyFramework.UserService.Dtos;
using FlyFramework.Utilities.JWTTokens;
using FlyFramework.Utilities.Redis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace FlyFramework.Controllers
{
    [ApiController]
    [DisabledUnitOfWork(true)]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ICacheManager _cacheManager;
        private readonly IJWTTokenManager _jWTTokenManager;
        private readonly IFlyFrameworkLazy<IUserManager> _userManagerLazy;
        private readonly IConfiguration _configuration;

        private IUserManager _userManager => _userManagerLazy.Value;
        public LoginController(SignInManager<User> signInManager,
            ICacheManager cacheManager,
            IJWTTokenManager jWTTokenManager,
            IStringLocalizerFactory factory,
            IConfiguration configuration,
            IFlyFrameworkLazy<IUserManager> userManagerLazy)
        {
            _signInManager = signInManager;
            _cacheManager = cacheManager;
            _jWTTokenManager = jWTTokenManager;
            _userManagerLazy = userManagerLazy;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<UserLoginDto> LoginIn(LoginDto input)
        {
            UserLoginDto userLoginDto = default;
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
                    new Claim(JwtRegisteredClaimNames.Sub, input.UserName),
                };
                var jwtBearer = _configuration.GetSection("JwtBearer").Get<JwtBearerModel>();

                var expireTime = DateTime.Now.AddMinutes(jwtBearer.AccessTokenExpiresMinutes);
                var token = _jWTTokenManager.GenerateToken(claims.ToList(), expireTime);

                userLoginDto = new UserLoginDto()
                {
                    AccessToken = token,
                    UserName = input.UserName,
                    NickName = user.FullName,
                    Expires = expireTime,
                    Roles = new List<string>()
                    {
                        "admin"
                    },
                    Permissions = new List<string>()
                    {
                        "*:*:*"
                    },
                    Avatar = "https://avatars.githubusercontent.com/u/44761321",
                    RefreshToken = _jWTTokenManager.GenerateToken(claims.ToList(), DateTime.Now.AddMinutes(jwtBearer.RefreshTokenExpiresMinutes))
                };
                if (input.IsApiLogin)
                {
                    Response.Cookies.Append("access-token", token, new CookieOptions()
                    {
                        Expires = expireTime
                    }
                    );
                }
            }
            return userLoginDto;
        }

        [HttpPost]
        public async Task<UserLoginDto> RefreshToken(string refreshToken)
        {
            UserLoginDto userLoginDto = default;
            var jwtBearer = _configuration.GetSection("JwtBearer").Get<JwtBearerModel>();
            if (refreshToken != null)
            {
                var claims = _jWTTokenManager.GetClaims(refreshToken);
                var userName = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                var user = await _userManager.FindByNameAsync(userName);

                var expireTime = DateTime.Now.AddMinutes(jwtBearer.AccessTokenExpiresMinutes);
                var newToken = _jWTTokenManager.GenerateToken(claims.ToList(), expireTime);
                await _cacheManager.SetCacheAsync(userName, newToken);
                userLoginDto = new UserLoginDto()
                {
                    AccessToken = newToken,
                    UserName = userName,
                    NickName = user.FullName,
                    Expires = DateTime.Now.AddMinutes(jwtBearer.AccessTokenExpiresMinutes),
                    Roles = new List<string>()
                    {
                        "admin"
                    },
                    Permissions = new List<string>()
                    {
                        "*:*:*"
                    },
                    Avatar = "https://avatars.githubusercontent.com/u/44761321",
                    RefreshToken = _jWTTokenManager.GenerateToken(claims.ToList(), DateTime.Now.AddMinutes(jwtBearer.RefreshTokenExpiresMinutes))
                };
            }
            return userLoginDto;
        }
    }
}

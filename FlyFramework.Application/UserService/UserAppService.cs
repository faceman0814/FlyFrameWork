using AutoMapper;

using FlyFramework.Application.UserService.Dtos;
using FlyFramework.Common.Extentions.DynamicWebAPI;
using FlyFramework.Core.UserService;
using FlyFramework.Core.UserService.DomainService;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FlyFramework.Application.UserService
{
    [Authorize]
    public class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IUserManager _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public UserAppService(IUserManager userManager, IMapper mapper, IConfiguration configuration, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        public async Task<User> GetUser(string id)
        {
            return await _userManager.FindById(id);
        }

        public async Task<string> LoginIn(UserDto input)
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
                return token;
            }
            else
            {
                return "登录失败";
            }

        }

        public string GenerateSecureKey()
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 128;  // 设置密钥长度为128位
                aesAlg.GenerateKey();  // 自动生成密钥
                return Convert.ToBase64String(aesAlg.Key);  // 将密钥转换为Base64字符串
            }
        }
        public async Task CreateUser(UserDto input)
        {
            var user = _mapper.Map<User>(input);
            user.NormalizedUserName = input.UserName.ToUpper();
            user.NormalizedEmail = input.Email.ToUpper();
            user.TwoFactorEnabled = false;
            user.EmailConfirmed = true;
            user.PhoneNumberConfirmed = false;
            await _userManager.Create(user);
        }
    }
}

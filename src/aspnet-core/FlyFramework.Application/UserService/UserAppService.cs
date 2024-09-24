using AutoMapper;

using FlyFramework.Application.UserService.Dtos;
using FlyFramework.Common.Utilities.JWTTokens;
using FlyFramework.Core.UserService;
using FlyFramework.Core.UserService.DomainService;
using FlyFramework.Domain.ApplicationServices;
using FlyFramework.Repositories.UserSessions;

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
        private readonly IJWTTokenManager _jwtTokenManager;
        private readonly IUserSession _userSession;

        public UserAppService(IServiceProvider serviceProvider,
            IUserManager userManager,
            IMapper mapper,
            IConfiguration configuration,
            SignInManager<User> signInManager,
            IUserSession userSession,
            IJWTTokenManager jWTTokenManager) : base(serviceProvider)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _signInManager = signInManager;
            _jwtTokenManager = jWTTokenManager;
            _userSession = userSession;
        }

        [AllowAnonymous]
        public string GenerateSecureKey()
        {
            Console.WriteLine(_userSession.UserId);
            Console.WriteLine(UserSession.UserId);
            using (var aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 128;  // 设置密钥长度为128位
                aesAlg.GenerateKey();  // 自动生成密钥
                return Convert.ToBase64String(aesAlg.Key);  // 将密钥转换为Base64字符串
            }
        }
        public async Task CreateUser(UserDto input)
        {
            var user = ObjectMapper.Map<User>(input);
            user.NormalizedUserName = input.UserName.ToUpper();
            user.NormalizedEmail = input.Email.ToUpper();
            user.TwoFactorEnabled = false;
            user.EmailConfirmed = true;
            user.PhoneNumberConfirmed = false;
            await _userManager.Create(user);
        }

        [HttpPost]
        public async Task UpdateUser(UserDto input)
        {
            var user = await _userManager.FindById(input.Id);
            user.Password = input.Password;
            await _userManager.Update(user);
        }
    }
}

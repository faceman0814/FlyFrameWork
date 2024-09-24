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

        public UserAppService(IServiceProvider serviceProvider,
            IUserManager userManager) : base(serviceProvider)
        {
            _userManager = userManager;
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

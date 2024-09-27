using AutoMapper;

using FlyFramework.Application.UserService.Dtos;
using FlyFramework.Common.ErrorExceptions;
using FlyFramework.Core.UserService;
using FlyFramework.Core.UserService.DomainService;
using FlyFramework.Domain.ApplicationServices;
using FlyFramework.Repositories.Uow;
using FlyFramework.Repositories.UserSessions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyFramework.Application.UserService
{
    [Authorize]
    public class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IUserManager _userManager;
        private readonly IUserSession _userSession;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserAppService(IServiceProvider serviceProvider
            , IUserManager userManager,
            IUnitOfWorkManager unitOfWorkManager,
            IUserSession userSession
            ) : base(serviceProvider)
        {
            _userManager = userManager;
            _userSession = userSession;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [AllowAnonymous]
        public void test()
        {
            Console.WriteLine(_userSession.UserId);
            Console.WriteLine(UserSession.UserId);
            ThrowUserFriendlyError("test");
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

        public async Task UpdateUser(UserDto input)
        {
            var user = await _userManager.FindById(input.Id);
            user.Password = input.Password;
            await _userManager.Update(user);
        }
    }
}

using FlyFramework.ApplicationServices;
using FlyFramework.UserService.DomainService;
using FlyFramework.UserService.Dtos;

using Microsoft.AspNetCore.Authorization;

using System;
using System.Threading.Tasks;
namespace FlyFramework.UserService
{
    [Authorize]
    public class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IUserManager _userManager;

        public UserAppService(IServiceProvider serviceProvider
            , IUserManager userManager
            ) : base(serviceProvider)
        {
            _userManager = userManager;
        }

        public async Task CreateUser(UserDto input)
        {
            var user = ObjectMapper.Map<User>(input);
            await _userManager.CreateUserAsync(user);
        }

        public async Task UpdateUser(UserDto input)
        {
            var user = await _userManager.FindByNameAsync(input.UserName);
            ObjectMapper.Map(input, user);
            await _userManager.Update(user);
        }
    }
}

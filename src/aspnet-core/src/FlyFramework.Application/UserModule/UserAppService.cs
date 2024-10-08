using FlyFramework.ApplicationServices;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.UserModule.DomainService;
using FlyFramework.UserModule.Dtos;

using Microsoft.AspNetCore.Authorization;

using System;
using System.Threading.Tasks;
namespace FlyFramework.UserModule
{
    [Authorize]
    public class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IUserManager _userManager;

        public UserAppService(IServiceProvider serviceProvider
            , IFlyFrameworkLazy flyFrameworkLazy
            , IUserManager userManager
            )
        {
            _userManager = flyFrameworkLazy.LazyGetRequiredService<IUserManager>().Value;
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

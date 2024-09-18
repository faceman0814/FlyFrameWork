using AutoMapper;

using FlyFramework.Application.UserService.Dtos;
using FlyFramework.Common.Extentions.DynamicWebAPI;
using FlyFramework.Core.UserService;
using FlyFramework.Core.UserService.DomainService;

using Microsoft.AspNetCore.Mvc;

namespace FlyFramework.Application.UserService
{
    //[Authorize]
    public class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IUserManager _userManager;
        private readonly IMapper _mapper;

        public UserAppService(IUserManager userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<User> GetUser(string id)
        {
            return await _userManager.FindById(id);
        }

        public async Task CreateUser(UserDto input)
        {
            var user = _mapper.Map<User>(input);
            await _userManager.Create(user);
        }
    }
}

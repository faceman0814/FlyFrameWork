using AutoMapper;

using FlyFramework.Application.UserService.Dtos;
using FlyFramework.Core.UserService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Application.UserService.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserDto>().ReverseMap(); // 配置User到UserDto的映射
        }

    }
}

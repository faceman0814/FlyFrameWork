using AutoMapper;

using FlyFramework.UserService.Dtos;

using GCT.MedPro.Application;

using Microsoft.AspNetCore.Builder;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.UserService.Mappers
{
    public class UserMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<User, UserDto>().ReverseMap().IgnoreNullSourceProperties();
        }
    }
}

using AutoMapper;

using FlyFramework.UserService.Dtos;

using GCT.MedPro.Application;

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

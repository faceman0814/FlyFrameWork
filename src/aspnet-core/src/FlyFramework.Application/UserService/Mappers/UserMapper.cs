using AutoMapper;

using FlyFramework.UserModule;
using FlyFramework.UserService.Dtos;

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

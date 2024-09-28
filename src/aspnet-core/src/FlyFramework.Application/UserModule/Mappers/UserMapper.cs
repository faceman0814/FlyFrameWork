using AutoMapper;

using FlyFramework.UserModule.Dtos;
using FlyFramework.UserService;

namespace FlyFramework.UserModule.Mappers
{
    public class UserMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<User, UserDto>().ReverseMap().IgnoreNullSourceProperties();
        }
    }
}

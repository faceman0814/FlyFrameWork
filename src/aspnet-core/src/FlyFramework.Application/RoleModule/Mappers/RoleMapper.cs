using AutoMapper;

using FlyFramework.RoleModule.Dtos;
using FlyFramework.UserModule;

namespace FlyFramework.RoleModule.Mappers
{
    public class RoleMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Role, RoleDto>().ReverseMap();
            configuration.CreateMap<RoleListDto, Role>().ReverseMap();
        }
    }
}

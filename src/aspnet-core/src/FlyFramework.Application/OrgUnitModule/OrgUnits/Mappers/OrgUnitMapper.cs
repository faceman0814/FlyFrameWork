using AutoMapper;

using FlyFramework.OrganizationalUnitModule;
using FlyFramework.OrgUnitModule.OrgUnits.Dtos;

using System.Collections.Generic;

namespace FlyFramework.OrgUnitModule.OrgUnits.Mappers
{
    public class OrgUnitMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<List<OrgUnit>, List<OrgUnitListDto>>().ReverseMap();
        }
    }
}

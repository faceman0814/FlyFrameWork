using AutoMapper;

using FlyFramework.OrganizationalUnitModule;
using FlyFramework.OrgUnitModule.OrgUnitNodes.Dtos;
using FlyFramework.UserModule;
using FlyFramework.UserModule.Dtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.OrgUnitNodes.Mappers
{
    public class OrgUnitNodeMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<List<OrgUnitNode>, List<OrgUnitNodeListDto>>().ReverseMap();
            configuration.CreateMap<OrgUnitNode, OrgUnitNodeEditDto>().ReverseMap().IgnoreNullSourceProperties();
        }
    }
}

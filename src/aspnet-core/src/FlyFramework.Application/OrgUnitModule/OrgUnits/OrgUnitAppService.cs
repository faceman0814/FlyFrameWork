using FlyFramework.ApplicationServices;
using FlyFramework.Dtos;
using FlyFramework.Extentions;
using FlyFramework.LazyModule.LazyDefinition;
using FlyFramework.OrganizationalUnitModule;
using FlyFramework.OrgUnitModule.DomainService.OrgUnits;
using FlyFramework.OrgUnitModule.OrgUnits.Dtos;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.OrgUnits
{
    public class OrgUnitAppService : ApplicationService, IOrgUnitAppService
    {
        private readonly IOrgUnitManager _orgUnitManager;
        private readonly IOrgUnitNodeManager _orgUnitNodeManager;

        public OrgUnitAppService(IFlyFrameworkLazy flyFrameworkLazy)
        {
            _orgUnitManager = flyFrameworkLazy.LazyGetRequiredService<IOrgUnitManager>().Value;
            _orgUnitNodeManager = flyFrameworkLazy.LazyGetRequiredService<IOrgUnitNodeManager>().Value;
        }

        /// <summary>
        /// 创建机构
        /// </summary>
        /// <returns></returns>
        [Authorize("123")]
        public async Task<string> Create(OrgUnitCreateInput input)
        {
            var orgUnit = ObjectMapper.Map<OrgUnit>(input);
            await _orgUnitManager.Create(orgUnit);
            return orgUnit.Id;
        }

        /// <summary>
        /// 查询子商信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(KnightSubAgencyPermissions.Query)]
        public async Task<PagedResultDto<OrgUnitListDto>> GetSubOrgUnitPaged(GetOrgUnitsInput input)
        {
            var nodeIdList = await _orgUnitNodeManager.GetGrantedNodes();

            var query = _orgUnitManager.QueryAsNoTracking
               .Where(t => nodeIdList.Contains(t.Id))
               .WhereIf(!input.FilterText.IsNullOrEmpty(), x => x.Name.Contains(input.FilterText))
               .Select(t => new OrgUnitListDto()
               {
                   Id = t.Id,
                   Name = t.Name,
                   CreationTime = t.CreationTime,
                   LastModificationTime = t.LastModificationTime,
               });

            var count = await query.CountAsync();

            var result = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<OrgUnitListDto>(count, result);
        }

    }
}

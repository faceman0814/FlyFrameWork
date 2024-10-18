using FlyFramework.Dtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.OrgUnits.Dtos
{
    public class GetOrgUnitsInput : PagedSortedAndFilteredInputDto
    {
        public string OrgUnitNodeId { get; set; }
        public string ParentOrgUnitNodeId { get; set; }
    }
}

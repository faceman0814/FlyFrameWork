using FlyFramework.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.OrgUnits.Dtos
{
    public class OrgUnitListDto : FullAuditedEntity<string>
    {
        public string Name { get; set; }
    }
}

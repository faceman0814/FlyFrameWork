using FlyFramework.OrganizationalUnitModule;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.OrgUnits.Dtos
{
    public class OrgUnitCreateInput
    {
        /// <summary>
        /// 组织机构名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父节点Id
        /// <see cref="OrgUnitNode.ParentId"/>
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 父级组织机构Id，aaa|bbb|ccc
        /// </summary>
        public string ParentIdList { get; set; }
    }
}

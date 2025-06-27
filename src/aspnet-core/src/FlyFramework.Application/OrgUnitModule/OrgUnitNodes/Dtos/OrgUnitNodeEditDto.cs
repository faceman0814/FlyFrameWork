using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.OrgUnitNodes.Dtos
{
    public class OrgUnitNodeEditDto
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        public string ParentIdList { get; set; }
        public string ParentId { get; set; }
    }
}

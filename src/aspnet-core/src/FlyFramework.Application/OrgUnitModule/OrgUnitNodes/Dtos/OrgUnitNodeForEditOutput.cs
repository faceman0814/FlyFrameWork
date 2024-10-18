using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.OrgUnitNodes.Dtos
{
    public class OrgUnitNodeForEditOutput
    {
        [Required]
        public OrgUnitNodeEditDto OrgUnitNode { get; set; }
    }
}

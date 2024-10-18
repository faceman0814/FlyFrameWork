using System.ComponentModel.DataAnnotations;

namespace FlyFramework.OrgUnitModule.OrgUnitNodes.Dtos
{
    public class CreateOrUpdateOrgUnitNodeInput
    {
        [Required]
        public OrgUnitNodeEditDto OrgUnitNode { get; set; }
    }
}

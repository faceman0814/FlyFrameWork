using FlyFramework.Dtos;

namespace FlyFramework.OrgUnitModule.OrgUnitNodes.Dtos
{
    public class GetOrgUnitNodesInput : PagedSortedAndFilteredInputDto
    {
        public string OrgUnitNodeId { get; set; }
        public string ParentOrgUnitNodeId { get; set; }
    }
}

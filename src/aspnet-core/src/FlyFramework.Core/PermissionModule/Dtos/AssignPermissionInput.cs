using System.Collections.Generic;

namespace FlyFramework.PermissionModule.Dtos
{
    public class AssignPermissionInput
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 分配类型
        /// </summary>
        public AssignPermissionEnum Type { get; set; }

        /// <summary>
        /// 权限Ids
        /// </summary>
        public List<string> PermissionIds { get; set; }
    }

    public enum AssignPermissionEnum
    {
        User,
        Role,
        OrgUnit
    }
}

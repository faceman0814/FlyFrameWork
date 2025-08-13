using System.Collections.Generic;

using static FlyFramework.PermissionModule.Permission;

namespace FlyFramework.PermissionModule.Dtos
{
    public class PermissionDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 权限Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 权限类型
        /// </summary>
        public PermissionType Type { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 子集
        /// </summary>
        public List<PermissionDto> Children { get; set; }
    }
}

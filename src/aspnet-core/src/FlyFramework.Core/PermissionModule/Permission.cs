using FlyFramework.Entities;

using Microsoft.AspNet.SignalR.Hubs;

using ServiceStack;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.PermissionModule
{
    /// <summary>
    /// 权限表
    /// </summary>
    [Table("permission")]
    public class Permission : Entity<string>
    {
        /// <summary>
        /// 权限Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 所属模块
        /// </summary>
        public string Module { get; set; }
        /// <summary>
        /// 权限类型
        /// </summary>
        public PermissionType Type { get; set; }
    }

    public enum PermissionType
    {
        [Description("操作权限")]
        Operation,
        [Description("数据权限")]
        Data
    }
}

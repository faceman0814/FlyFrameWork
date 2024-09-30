using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Authorizations.JwtBearer
{
    /// <summary>
    /// 登录声明类型帮助类，定义了常用的声明类型名称。
    /// </summary>
    public static class UserClaimTypes
    {
        /// <summary>
        /// 用户名声明类型，默认值为 <see cref="ClaimTypes.Name"/>。
        /// </summary>
        public static string UserName { get; set; } = ClaimTypes.Name;

        /// <summary>
        /// 过期时间声明类型，默认值为 <see cref="ClaimTypes.Expiration"/>。
        /// </summary>
        public static string Expiration { get; set; } = ClaimTypes.Expiration;

        /// <summary>
        /// 用户 ID 声明类型，默认值为 <see cref="ClaimTypes.NameIdentifier"/>。
        /// </summary>
        public static string UserId { get; set; } = ClaimTypes.NameIdentifier;

        /// <summary>
        /// 角色声明类型，默认值为 <see cref="ClaimTypes.Role"/>。
        /// </summary>
        public static string Role { get; set; } = ClaimTypes.Role;
        /// <summary>
        /// 租户 ID 声明类型，默认值为 "TenantId"。
        /// </summary>
        public static string TenantId { get; set; } = "TenantId";
        /// <summary>
        /// 租户名称声明类型，默认值为 "TenantName"。
        /// </summary>
        public static string TenantName { get; set; } = "TenantName";
    }
}

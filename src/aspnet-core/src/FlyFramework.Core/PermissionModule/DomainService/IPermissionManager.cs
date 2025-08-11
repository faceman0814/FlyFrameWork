using FlyFramework.Domains;
using FlyFramework.PermissionModule.Dtos;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFramework.PermissionModule.DomainService
{
    public interface IPermissionManager : IGuidDomainService<Permission>
    {
        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <returns></returns>
        Task<List<PermissionDto>> GetAllPermission();
    }
}

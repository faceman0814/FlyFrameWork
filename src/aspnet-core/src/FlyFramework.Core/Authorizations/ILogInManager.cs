using FlyFramework.Dependencys;
using FlyFramework.UserService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Authorizations
{
    public interface ILogInManager : ITransientDependency
    {
        /// <summary>
        /// 获取登录用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ClaimsIdentity> LoginAsync(User user);
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        IEnumerable<Claim> GetClaims(string token);
    }
}

﻿using FlyFramework.Domains;
using FlyFramework.UserService;

using System.Threading.Tasks;
namespace FlyFramework.UserService.DomainService
{
    public interface IUserManager : IGuidDomainService<User>
    {
        /// <summary>
        /// 根据用户名查找用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<User> FindByNameAsync(string userName);
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<User> CreateUserAsync(User user);
    }
}

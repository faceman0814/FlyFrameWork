using FaceMan.DynamicWebAPI;

using FlyFramework.ApplicationServices;
using FlyFramework.UserModule;
using FlyFramework.Utilities.Dappers;

using System;
using System.Threading.Tasks;

namespace FlyFramework.Test
{
    /// <summary>
    /// Dapper 应用服务
    /// </summary>
    public class DapperAppService : ApplicationService, IApplicationService
    {
        private readonly IDapperManager<User> _dapperManager;
        public DapperAppService(IServiceProvider serviceProvider, IDapperManager<User> dapperManager)
        {
            _dapperManager = dapperManager;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns></returns>
        public async Task<User> GetAsync(string Id)
        {
            var result = await _dapperManager.GetByIdAsync(Id);
            return result;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(string Id)
        {
            await _dapperManager.ExecuteAsync($"Update  [User] Set FullName='admin' where Id='{Id}'");
        }
    }
}

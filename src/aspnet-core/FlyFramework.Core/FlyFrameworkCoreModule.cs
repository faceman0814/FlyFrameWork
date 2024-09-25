
using FlyFramework.Common.FlyFrameworkModules;
using FlyFramework.Common.FlyFrameworkModules.Modules;

namespace FlyFramework.Core
{
    public class FlyFrameworkCoreModule : BaseModule
    {
        /// <summary>
        /// 预处理程序
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void PreConfigureServices(ServiceConfigerContext context)
        {
        }

        /// <summary>
        /// 服务注册与配置
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void ConfigerService(ServiceConfigerContext context)
        {
        }

        /// <summary>
        /// 配置中间件
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void InitApplication(InitApplicationContext context)
        {
        }
    }
}

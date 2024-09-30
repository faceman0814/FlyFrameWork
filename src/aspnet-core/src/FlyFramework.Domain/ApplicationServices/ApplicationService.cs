using FlyFramework.Attributes;
using FlyFramework.Dependencys;
using FlyFramework.ErrorExceptions;
using FlyFramework.UserSessions;

using System;
namespace FlyFramework.ApplicationServices
{
    public abstract class ApplicationService : ApplicationServiceBase, IApplicationService, ITransientDependency
    {
        /// <summary>
        /// API 通用后缀
        /// </summary>
        public static string[] CommonPostfixes { get; set; } = { "AppService", "ApplicationService", "Service" };

        /// <summary>
        /// 用户信息
        /// </summary>
        [IocSelect]
        public IUserSession UserSession { get; set; }


        public ApplicationService(IServiceProvider serviceProvider, string localizationSourceName = null)
        {
            LocalizationSourceName = localizationSourceName ?? FlyFrameworkConsts.LocalizationSourceName;
        }

        protected virtual void ThrowUserFriendlyError(string reason)
        {
            throw new UserFriendlyException(L("Error"), L("UserFriendlyError", reason, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        }
    }
}

using FlyFramework.Attributes;
using FlyFramework.Dependencys;
using FlyFramework.ErrorExceptions;
using FlyFramework.UserSessions;

using Microsoft.Extensions.DependencyInjection;

using System;
namespace FlyFramework.ApplicationServices
{
    public abstract class ApplicationService : ApplicationServiceBase, IApplicationService, ITransientDependency
    {
        /// <summary>
        /// API 通用后缀
        /// </summary>
        public static string[] CommonPostfixes { get; set; } = { "AppService", "ApplicationService", "Service" };

        public ApplicationService(string localizationSourceName = null)
        {
            LocalizationSourceName = localizationSourceName ?? FlyFrameworkConsts.LocalizationSourceName;
        }

        protected virtual void ThrowUserFriendlyError(string reason)
        {
            throw new UserFriendlyException(L("Error"), L("UserFriendlyError", reason, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        }

        public TService GetService<TService>()
        {
            return this.ServiceProvider.GetRequiredService<TService>();
        }
    }
}

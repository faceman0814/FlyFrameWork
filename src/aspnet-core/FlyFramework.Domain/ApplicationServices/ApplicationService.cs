using AutoMapper;

using FlyFramework.Repositories.UserSessions;

using Microsoft.Extensions.DependencyInjection;

namespace FlyFramework.Domain.ApplicationServices
{
    public abstract class ApplicationService : ApplicationServiceBase, IApplicationService
    {
        /// <summary>
        /// API 通用后缀
        /// </summary>
        public static string[] CommonPostfixes { get; set; } = { "AppService", "ApplicationService", "Service" };

        /// <summary>
        /// 用户信息
        /// </summary>
        //[IocSelect]
        public IUserSession UserSession { get; set; }


        public ApplicationService(IServiceProvider serviceProvider)
        {
            ObjectMapper = serviceProvider.GetRequiredService<IMapper>();
            UserSession = serviceProvider.GetRequiredService<IUserSession>();
        }
        //private ILocalizationSource _localizationSource;

    }
}

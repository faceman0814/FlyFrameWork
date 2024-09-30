using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework
{
    public class FlyFrameworkConst
    {
        public static class Fields
        {
            //
            // 摘要:
            //     Guid最大长度
            public const int GuidMaxCount = 20;

            //
            // 摘要:
            //     备注 最大长度
            public const int RemarkMaxCount = 2000;
        }

        public const string DynamicMenuConfigFileName = "Menu.json";

        public const string DynamicPermissionWebApiFileName = "WebApis.json";

        public const string DynamicPermisisonFileName = "Permissions.json";

        public const string DynamicPdaMenuConfigFileName = "PdaMenu.json";

        //
        // 摘要:
        //     教师角色
        public const string TeacherRoleName = "Teacher";

        //
        // 摘要:
        //     学生角色
        public const string StudentRoleName = "Student";

        //
        // 摘要:
        //     Admin角色
        public const string AdminRoleName = "Admin";

        public static string SuperAdmin = "SuperAdmin";

        //
        // 摘要:
        //     MVC中域的常量名称
        public const string AreasAdminName = "AreasAdminName";

        //
        // 摘要:
        //     ShardingCore默认数据源名称
        public const string DefaultDataSourceName = "Default";

        //
        // 摘要:
        //     门户端的管理中心常量
        public const string PortalAdminName = "Admin";

        //
        // 摘要:
        //     语言文件的名称
        public const string LocalizationSourceName = "YoyoBoot";

        //
        // 摘要:
        //     默认语言
        public const string DefaultLanguage = "zh-Hans";

        //
        // 摘要:
        //     多租户请求头名称
        public const string TenantIdResolveKey = "Tenant";

        //
        // 摘要:
        //     默认一页显示多少条数据
        public const int DefaultPageSize = 10;

        //
        // 摘要:
        //     一次性最多可以分多少页
        public const int MaxPageSize = 1000;

        //
        // 摘要:
        //     邮件地址最大长度
        public const int MaxEmailAddressLength = 250;

        //
        // 摘要:
        //     个人头像图片大小最多不超过1M
        public const int ResizedMaxProfilPictureBytesUserFriendlyValue = 1024;

        //
        // 摘要:
        //     文件最大不超过50M
        public static long MaxFileSize = 52428800L;

        //
        // 摘要:
        //     名字最大长度
        public const int MaxNameLength = 50;

        public const int MaxAddressLength = 250;

        public const int PaymentCacheDurationInMinutes = 30;

        //
        // 摘要:
        //     Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        public const string DefaultPassPhrase = "gsKxGZ012HLL3MI5";

        public const int MaxProfilePictureBytesUserFriendlyValue = 5;

        public const int MaxListContentLength = 150;

        public const string PageviewKey = "PageviewKey";

        //
        // 摘要:
        //     表的前缀名称
        public const string TablePrefix = "Ltm";

        public const string Currency = "USD";

        public const string CurrencySign = "$";

        public const string AddressLinkagePacsPath = "pacs-code.json";

        public const string AddressLinkageProvincesPath = "provinces.json";

        public const string AddressLinkageCitiesPath = "cities.json";

        public const string AddressLinkageAreasPath = "areas.json";

        public const string AddressLinkageStreetsPath = "streets.json";

        //
        // 摘要:
        //     时间单位 分钟
        public const string TimeUnitMinute = "TimeUnitMinute";

        //
        // 摘要:
        //     小时单位 小时
        public const string TimeUnitHour = "TimeUnitHour";

        //
        // 摘要:
        //     时间单位 天
        public const string TimeUnitDay = "TimeUnitDay";

        //
        // 摘要:
        //     时间单位 月
        public const string TimeUnitMonth = "TimeUnitMonth";

        //
        // 摘要:
        //     下载信息Session
        public const string DownloadFileCode = "DownloadFileCode";

        public const string TokenType = "token_type";

        public const string ClientType = "client_type";

        public const string ClientTokenTag = "client_token_tag";

        public const string TokenValidityKey = "token_validity_key";

        public const string RefreshTokenValidityKey = "refresh_token_validity_key";

        public static string UserIdentifier = "user_identifier";
        //
        // 摘要:
        //     水印名称
        public const string WaterMarkName = "watermark";

        public const string EchartsData = "EchartsData";

        /// <summary>
        /// Token过期时间 默认1天
        /// </summary>
        public static TimeSpan AccessTokenExpiration = TimeSpan.FromDays(1);

        /// <summary>
        /// RefreshToken过期时间 默认30h
        /// </summary>
        public static TimeSpan RefreshTokenExpiration = TimeSpan.FromHours(30);
    }
}
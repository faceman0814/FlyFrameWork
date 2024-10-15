using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework
{
    public static class FlyFrameworkConfigs
    {
        /// <summary>
        /// EFCore配置
        /// </summary>
        public static class Database
        {
            /// <summary>
            /// 跳过DbContext注册
            /// </summary>
            public static bool SkipDbContextRegistration { get; set; }

            /// <summary>
            /// 跳过种子数据
            /// </summary>
            public static bool SkipDbSeed { get; set; }
        }

        /// <summary>
        /// 排序
        /// </summary>
        public static class Sorting
        {
            /// <summary>
            /// 默认 CreationTime Desc
            /// </summary>
            public static string Default = "CreationTime Desc";

            public static string Id = "Id";
        }
    }
}

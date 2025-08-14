using Autofac;

using FlyFramework.FlyFrameworkModules;
using FlyFramework.FlyFrameworkModules.Interface;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Reflection;

using Module = Autofac.Module;


namespace FlyFramework.FlyFrameworkModules.Modules
{
    public abstract class FlyFrameworkBaseModule : Module, IFlyFrameworkBaseModule
    {

        protected internal ServiceConfigerContext Configuration
        {
            get
            {
                if (_configserviceContext is null)
                {
                    throw new ArgumentException("注册服务时未对_ConfigServiceContext赋值");
                }
                return _configserviceContext;
            }
            internal set => _configserviceContext = value;
        }

        private ServiceConfigerContext _configserviceContext;

        /// <summary>
        /// 预处理程序
        /// </summary>
        public virtual void PreInitialize()
        {

        }

        /// <summary>
        /// 服务注册与配置
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// 配置中间件
        /// </summary>
        public virtual void InitApplication()
        {
        }

        /// <summary>
        /// 加载后处理程序
        /// </summary>
        /// <param name="context"></param>
        public virtual void PostInitialize()
        {

        }

        public static bool IsModule(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return
                typeInfo.IsClass &&
                !typeInfo.IsAbstract &&
                !typeInfo.IsGenericType &&
                typeof(IFlyFrameworkBaseModule).GetTypeInfo().IsAssignableFrom(type);
        }

        internal static void CheckModuleType(Type type)
        {
            if (!IsModule(type))
            {
                throw new ArgumentNullException($"{type.Name}没有继承BaseModule");
            }
        }

        public void Configure<TOptions>(Action<TOptions> action) where TOptions : class
        {
            Configuration.Services.Configure(action);
        }
    }
}
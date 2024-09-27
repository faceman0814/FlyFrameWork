using System;

namespace FlyFramework.Common.FlyFrameworkModules
{
    public class InitApplicationContext
    {
        public IServiceProvider ServiceProvider { get; set; }

        public InitApplicationContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
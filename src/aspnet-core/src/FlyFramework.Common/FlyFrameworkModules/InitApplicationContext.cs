using System;

namespace FlyFramework.FlyFrameworkModules
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
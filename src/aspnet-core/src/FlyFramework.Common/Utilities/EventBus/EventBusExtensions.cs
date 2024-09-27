using FlyFramework.Common.Utilities.EventBus.MediatR;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Utilities.EventBus
{
    public static class EventBusExtensions
    {
        public static IServiceCollection AddLocalEventBus(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                                    .Where(x => !x.Contains("Microsoft.") && !x.Contains("System."))
                                    .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x))).ToArray());
                cfg.NotificationPublisher = new FlyFrameworkPublisher();
                cfg.NotificationPublisherType = typeof(FlyFrameworkPublisher);
            });
            return services;
        }

    }
}

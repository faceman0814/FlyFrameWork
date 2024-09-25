using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace FlyFramework.Common.Localizations;

public static class JsonLocalizationExtensions
{
    /// <summary>
    /// Adds services required for application localization.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddJsonLocalization(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new Exception();
        }
        services.AddOptions();
        services.AddLogging();
        services.PostConfigure<JsonLocalizationOptions>(options => { options.ResourcesPath ??= "Resources"; });
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        services.TryAddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

        return services;
    }

    /// <summary>
    /// Adds services required for application localization.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="setupAction">
    /// An <see cref="Action{LocalizationOptions}"/> to configure the <see cref="JsonLocalizationOptions"/>.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptions> setupAction)
    {
        if (services == null || setupAction == null)
        {
            throw new Exception();
        }
        services.Configure(setupAction);
        services.AddJsonLocalization();

        return services;
    }
}

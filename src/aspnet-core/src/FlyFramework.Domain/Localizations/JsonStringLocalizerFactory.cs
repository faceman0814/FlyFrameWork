﻿using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Concurrent;
using System.Reflection;
namespace FlyFramework.Localizations;

internal sealed class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly ConcurrentDictionary<string, JsonStringLocalizer> _localizerCache = new();
    private readonly ILogger _logger;
    private readonly JsonLocalizationOptions _localizationOptions;

    public JsonStringLocalizerFactory(IOptions<JsonLocalizationOptions> localizationOptions, ILoggerFactory loggerFactory)
    {
        _localizationOptions = localizationOptions.Value;
        _logger = loggerFactory.CreateLogger<JsonStringLocalizer>();
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        if (resourceSource == null)
        {
            throw new Exception();
        }
        var resourceName = TrimPrefix(resourceSource.FullName, (_localizationOptions.RootNamespace ?? Assembly.GetEntryAssembly()?.GetName().Name ?? AppDomain.CurrentDomain.FriendlyName) + ".");
        return CreateJsonStringLocalizer(resourceName);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        if (baseName == null || location == null)
        {
            throw new Exception();
        }
        var resourceName = TrimPrefix(baseName, location + ".");
        return CreateJsonStringLocalizer(resourceName);
    }

    private JsonStringLocalizer CreateJsonStringLocalizer(string resourceName)
    {
        _logger.LogInformation("Looking for resource: {resourceName}", resourceName);
        return _localizerCache.GetOrAdd(resourceName, resName => new JsonStringLocalizer(
            _localizationOptions,
            resName,
            _logger));
    }

    private static string TrimPrefix(string name, string prefix)
    {
        return name.StartsWith(prefix, StringComparison.Ordinal)
                ? name.Substring(prefix.Length)
                : name
            ;
    }
}
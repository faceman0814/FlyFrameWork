using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
namespace FlyFramework.Localizations;

internal sealed class JsonStringLocalizer : IStringLocalizer
{
    private readonly ConcurrentDictionary<string, Dictionary<string, string>> _resourcesCache = new();
    private readonly string _resourcesPath;
    private readonly string _resourceName;
    private readonly ResourcesPathType _resourcesPathType;
    private readonly ILogger _logger;

    private string _searchedLocation;

    public JsonStringLocalizer(
        JsonLocalizationOptions localizationOptions,
        string resourceName,
        ILogger logger)
    {
        _resourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
        _logger = logger ?? NullLogger.Instance;
        _resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, localizationOptions.ResourcesPath);
        _resourcesPathType = localizationOptions.ResourcesPathType;
    }

    public LocalizedString this[string name]
    {
        get
        {
            if (name == null)
            {
                throw new Exception();
            }
            var value = GetStringSafely(name);
            return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _searchedLocation);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            if (name == null)
            {
                throw new Exception();
            }
            var format = GetStringSafely(name);
            var value = string.Format(format ?? name, arguments);
            return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: _searchedLocation);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        GetAllStrings(includeParentCultures, CultureInfo.CurrentUICulture);

    public IStringLocalizer WithCulture(CultureInfo culture) => this;

    private IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, CultureInfo culture)
    {
        if (culture == null)
        {
            throw new Exception();
        }
        var resourceNames = includeParentCultures
            ? GetAllStringsFromCultureHierarchy(culture)
            : GetAllResourceStrings(culture);

        foreach (var name in resourceNames)
        {
            var value = GetStringSafely(name);
            yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _searchedLocation);
        }
    }

    private string GetStringSafely(string name)
    {
        if (name == null)
        {
            throw new Exception();
        }
        string value = null;

        var resources = GetResources(CultureInfo.CurrentUICulture.Name);
        if (resources?.TryGetValue(name, out var resource) is true)
        {
            value = resource;
        }

        return value;
    }

    private IEnumerable<string> GetAllStringsFromCultureHierarchy(CultureInfo startingCulture)
    {
        var currentCulture = startingCulture;
        var resourceNames = new HashSet<string>();

        while (currentCulture.Equals(currentCulture.Parent) == false)
        {
            var cultureResourceNames = GetAllResourceStrings(currentCulture);

            if (cultureResourceNames != null)
            {
                foreach (var resourceName in cultureResourceNames)
                {
                    resourceNames.Add(resourceName);
                }
            }

            currentCulture = currentCulture.Parent;
        }

        return resourceNames;
    }

    private IEnumerable<string> GetAllResourceStrings(CultureInfo culture)
    {
        var resources = GetResources(culture.Name);
        return resources?.Select(r => r.Key);
    }
    private string GetSubstringAfterLastDot(string input)
    {
        if (input == null) return null;
        int lastIndex = input.LastIndexOf('.');
        if (lastIndex == -1) return input; // 如果没有点，返回原字符串
        return input.Substring(lastIndex + 1);
    }
    private Dictionary<string, string> GetResources(string culture)
    {
        var res = _resourcesCache.GetOrAdd(culture, _ =>
        {
            var resourceFile = "json";
            var resourceName = GetSubstringAfterLastDot(_resourceName);
            if (_resourcesPathType == ResourcesPathType.TypeBased)
            {
                resourceFile = $"{culture}.json";
                if (resourceName != null)
                {
                    resourceFile = string.Join(".", resourceName.Replace('.', Path.DirectorySeparatorChar), resourceFile);
                }
            }
            else
            {
                resourceFile = string.Join(".",
                    Path.Combine(culture, resourceName.Replace('.', Path.DirectorySeparatorChar)), resourceFile);
            }

            _searchedLocation = Path.Combine(_resourcesPath, resourceFile);
            Dictionary<string, string> value = null;

            if (File.Exists(_searchedLocation))
            {
                try
                {
                    using var stream = File.OpenRead(_searchedLocation);
                    value = JsonSerializer.Deserialize<Dictionary<string, string>>(stream);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to get json content, path: {path}", _searchedLocation);
                }
            }
            else
            {
                _logger.LogWarning("Resource file {path} not exists", _searchedLocation);
            }

            return value;
        });
        return res;
    }
}

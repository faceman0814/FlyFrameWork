using Masuit.Tools;

using Microsoft.Extensions.Localization;

using System.Globalization;

namespace FlyFramework.Domain.Localizations
{
    public class LocalizationSource : ILocalizationSource
    {
        public string Name { get; }
        private readonly IStringLocalizer _localizations;
        public LocalizationSource(string name, IStringLocalizerFactory factory, Type type)
        {
            Name = name;
            _localizations = factory.Create(name, type.Name);
        }

        public string GetString(string name)
        {
            return GetStringOrNull(name) ?? throw new KeyNotFoundException($"Key '{name}' not found.");
        }

        public string GetString(string name, CultureInfo culture)
        {
            return GetStringOrNull(name, culture) ?? throw new KeyNotFoundException($"Key '{name}' for culture '{culture.Name}' not found.");
        }

        public string GetStringOrNull(string name, bool tryDefaults = true)
        {
            var value = _localizations.GetString(name).Value;
            if (!value.IsNullOrEmpty())
            {
                return value;
            }

            if (tryDefaults && CultureInfo.CurrentCulture != CultureInfo.InvariantCulture)
            {
                return GetStringOrNull($"{name}-{CultureInfo.CurrentCulture.Name}", false);
            }

            return null;
        }

        public string GetStringOrNull(string name, CultureInfo culture, bool tryDefaults = true)
        {
            var key = $"{name}-{culture.Name}";
            var value = _localizations.GetString(key).Value;
            if (!value.IsNullOrEmpty())
            {
                return value;
            }

            if (tryDefaults && culture != CultureInfo.InvariantCulture)
            {
                return GetStringOrNull(name);
            }

            return null;
        }

        public List<string> GetStrings(List<string> names)
        {
            var results = new List<string>();
            foreach (var name in names)
            {
                results.Add(GetString(name));
            }
            return results;
        }

        public List<string> GetStrings(List<string> names, CultureInfo culture)
        {
            var results = new List<string>();
            foreach (var name in names)
            {
                results.Add(GetString(name, culture));
            }
            return results;
        }

        public List<string> GetStringsOrNull(List<string> names, bool tryDefaults = true)
        {
            var results = new List<string>();
            foreach (var name in names)
            {
                results.Add(GetStringOrNull(name, tryDefaults));
            }
            return results;
        }

        public List<string> GetStringsOrNull(List<string> names, CultureInfo culture, bool tryDefaults = true)
        {
            var results = new List<string>();
            foreach (var name in names)
            {
                results.Add(GetStringOrNull(name, culture, tryDefaults));
            }
            return results;
        }

        public IReadOnlyList<LocalizedString> GetAllStrings(bool includeDefaults = true)
        {
            var results = new List<LocalizedString>();
            foreach (var pair in _localizations.GetAllStrings())
            {
                results.Add(new LocalizedString(pair.Value, ExtractCultureNameFromKey(pair.Name)));
            }
            return results;
        }

        public IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture, bool includeDefaults = true)
        {
            var results = new List<LocalizedString>();
            var cultureSuffix = $"-{culture.Name}";
            foreach (var pair in _localizations.GetAllStrings())
            {
                if (pair.Name.EndsWith(cultureSuffix) || includeDefaults && !pair.Name.Contains("-"))
                {
                    results.Add(new LocalizedString(pair.Value, ExtractCultureNameFromKey(pair.Name)));
                }
            }
            return results;
        }

        private string ExtractCultureNameFromKey(string key)
        {
            var parts = key.Split('-');
            return parts.Length > 1 ? parts[1] : "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Domain.Localizations
{
    public class LocalizationManager : ILocalizationManager
    {
        private readonly Dictionary<string, ILocalizationSource> _sources;

        public LocalizationManager(ILocalizationSource localizationSource)
        {
            _sources = new Dictionary<string, ILocalizationSource>();
            AddSource(localizationSource.Name, localizationSource);
        }

        public ILocalizationSource GetSource(string name)
        {
            if (_sources.TryGetValue(name, out var source))
            {
                return source;
            }
            throw new ArgumentException($"未能找到名为 '{name}' 的本地化资源。");
        }

        public IReadOnlyList<ILocalizationSource> GetAllSources()
        {
            return _sources.Values.ToList().AsReadOnly();
        }

        public void AddSource(string name, ILocalizationSource source)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("资源名称不能为空。");
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "本地化资源不能为空。");
            }

            if (_sources.ContainsKey(name))
            {
                throw new ArgumentException($"已存在名为 '{name}' 的本地化资源。");
            }

            _sources[name] = source;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Localizations
{
    public interface ILocalizationManager
    {
        ILocalizationSource GetSource(string name);
        IReadOnlyList<ILocalizationSource> GetAllSources();
    }
}

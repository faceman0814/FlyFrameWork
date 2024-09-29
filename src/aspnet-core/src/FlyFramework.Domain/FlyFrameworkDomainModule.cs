using FlyFramework.FlyFrameworkModules.Modules;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework
{
    [DependOn(typeof(FlyFrameworkCommonModule), typeof(FlyFrameworkRepositoriesModule))]
    public class FlyFrameworkDomainModule : FlyFrameworkBaseModule
    {
    }
}

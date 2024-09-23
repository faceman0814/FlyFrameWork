using FlyFramework.Application.DynamicWebAPI;
using FlyFramework.Common.Utilities.Dappers;
using FlyFramework.Core.UserService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Application.Test
{
    public class DapperAppService : ApplicationService, IApplicationService
    {
        private readonly IDapperManager<User> _dapperManager;
        public DapperAppService(IDapperManager<User> dapperManager)
        {
            _dapperManager = dapperManager;
        }

        public async Task<User> GetAsync()
        {
            var result = await _dapperManager.GetByIdAsync("4957adb8870f4e79882231537ff5d3b9");
            return result;
        }
    }
}

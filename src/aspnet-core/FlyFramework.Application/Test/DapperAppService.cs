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

        public async Task<User> GetAsync(string Id)
        {
            var result = await _dapperManager.GetByIdAsync(Id);
            return result;
        }

        public async Task ExecuteAsync(string Id)
        {
            await _dapperManager.ExecuteAsync($"Update  [User] Set FullName='Admin' where Id='{Id}'");
        }
    }
}

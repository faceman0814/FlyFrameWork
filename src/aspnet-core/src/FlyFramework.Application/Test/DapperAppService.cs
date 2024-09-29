using FlyFramework.ApplicationServices;
using FlyFramework.UserService;
using FlyFramework.Utilities.Dappers;

using System;
using System.Threading.Tasks;

namespace FlyFramework.Test
{
    public class DapperAppService : ApplicationService, IApplicationService
    {
        private readonly IDapperManager<User> _dapperManager;
        public DapperAppService(IServiceProvider serviceProvider, IDapperManager<User> dapperManager)
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

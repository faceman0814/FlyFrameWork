using FlyFramework.Common.Utilities.Dappers;
using FlyFramework.Core.UserService;
using FlyFramework.Domain.ApplicationServices;

using System;
using System.Threading.Tasks;

namespace FlyFramework.Application.Test
{
    public class DapperAppService : ApplicationService, IApplicationService
    {
        private readonly IDapperManager<User> _dapperManager;
        public DapperAppService(IServiceProvider serviceProvider, IDapperManager<User> dapperManager) : base(serviceProvider)
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

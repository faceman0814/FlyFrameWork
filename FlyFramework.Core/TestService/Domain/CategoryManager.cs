using FlyFramework.Common.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Core.TestService.Domain
{
    public class CategoryManager : DomainService<Category, string>, ICategoryManager
    {
        public CategoryManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override IQueryable<Category> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override Task ValidateOnCreateOrUpdate(Category entity)
        {
            return Task.CompletedTask;
        }

        public override Task ValidateOnDelete(Category entity)
        {
            return Task.CompletedTask;
        }
    }
}

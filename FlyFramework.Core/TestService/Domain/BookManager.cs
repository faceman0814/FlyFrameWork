using FlyFramework.Common.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Core.TestService.Domain
{
    public class BookManager : DomainService<Book, string>, IBookManager
    {
        public BookManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override IQueryable<Book> GetIncludeQuery()
        {
            throw new NotImplementedException();
        }

        public override Task ValidateOnCreateOrUpdate(Book entity)
        {
            return Task.CompletedTask;
        }

        public override Task ValidateOnDelete(Book entity)
        {
            return Task.CompletedTask;
        }
    }
}

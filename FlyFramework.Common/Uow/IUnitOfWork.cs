using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Uow
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}

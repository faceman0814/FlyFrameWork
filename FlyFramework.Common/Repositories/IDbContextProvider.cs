using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Repositories
{
    public interface IDbContextProvider
    {
        DbContext GetDbContext();
    }
}

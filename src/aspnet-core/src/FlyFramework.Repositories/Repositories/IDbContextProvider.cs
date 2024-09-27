using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Repositories.Repositories
{
    public interface IDbContextProvider
    {
        DbContext GetDbContext();
    }
}

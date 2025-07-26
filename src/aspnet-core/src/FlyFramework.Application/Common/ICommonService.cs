using FlyFramework.Dtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common
{
    public interface ICommonService
    {
        Task<List<ColumnDto>> GetColumnList<T>() where T : class;

    }
}

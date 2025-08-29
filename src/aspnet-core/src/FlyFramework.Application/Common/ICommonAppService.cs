using FaceMan.DynamicWebAPI;

using FlyFramework.Dtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common
{
    public interface ICommonAppService: IApplicationService
    {
        Task<List<ColumnDto>> GetColumnList<T>() where T : class;
        Task<List<ColumnDto>> GetColumns(string type);
    }
}

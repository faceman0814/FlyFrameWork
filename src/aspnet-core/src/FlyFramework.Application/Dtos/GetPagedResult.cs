using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Dtos
{
    public class GetPagedResult<T> where T : class
    {
        public List<ColumnDto> columns { get; set; }
        public PagedResultDto<T> datas { get; set; }
    }
}

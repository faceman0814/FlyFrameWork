using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Dtos
{
    public class GetDropDownListInput : PagedSortedAndFilteredInputDto
    {
        /// <summary>
        /// 已选的ids值
        /// </summary>
        public List<string> Ids { get; set; }
    }
}

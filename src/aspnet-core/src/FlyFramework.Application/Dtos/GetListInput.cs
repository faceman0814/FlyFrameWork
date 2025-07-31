using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Dtos
{
    public class GetListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        /// <summary>
		/// 正常化排序使用
		/// </summary>
		public virtual void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime desc";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework
{
    public interface IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 主键
        /// </summary>
        TPrimaryKey Id { get; set; }

        string ConcurrencyToken { get; set; }

        /// <summary>
        /// 检查该实体是否是瞬态（不持续到数据库）
        /// </summary>
        /// <returns></returns>
        bool IsTransient();
    }
}

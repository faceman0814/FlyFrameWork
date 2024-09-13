using FlyFramework.Common.Entities;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Core.TestService
{
    public class Category : IEntity<string>
    {
        public Category()
        {
            Name = string.Empty;
            Code = string.Empty;
        }

        /// <summary>
        /// 分类代码
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Code { get; set; }

        /// <summary>
        /// 分类名
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        //导航属性
        public virtual IList<Book> Books { get; set; }
        public string Id { get; set; }

        public bool IsTransient()
        {
            throw new NotImplementedException();
        }
    }
}

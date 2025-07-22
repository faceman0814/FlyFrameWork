using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Entities
{
    [Serializable]
    public class AuditedEntity<TPrimaryKey> : CreationAuditedEntity<TPrimaryKey>, IAuditedEntity<TPrimaryKey>
    {
        /// <summary>
        /// 最后修改时间
        /// </summary>
        [Comment("最后修改时间")]
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 最后修改人名称
        /// </summary>
        [Comment("最后修改人名称")]
        public virtual string LastModifierUserName { get; set; }

        /// <summary>
        /// 最后修改人Id
        /// </summary>
        [MaxLength(32)]
        [Comment("最后修改人Id")]
        public virtual TPrimaryKey LastModifierUserId { get; set; }
    }
}

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
    public abstract class CreationAuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, ICreationAuditedEntity<TPrimaryKey>
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [Comment("创建时间")]
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        [Comment("创建人名称")]
        public virtual string CreatorUserName { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        [Comment("创建人Id")]
        [MaxLength(32)]
        public virtual TPrimaryKey CreatorUserId { get; set; }

        protected CreationAuditedEntity()
        {
            CreationTime = DateTime.Now;
        }
    }
}

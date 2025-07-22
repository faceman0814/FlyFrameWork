using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations;
namespace FlyFramework.Entities
{
    public class FullAuditedEntity<TPrimaryKey> : AuditedEntity<TPrimaryKey>, IFullAuditedEntity<TPrimaryKey>
    {
        /// <summary>
        /// 是否已删除
        /// </summary>
        [Comment("是否已删除")]
        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// 删除人Id
        /// </summary>
        [MaxLength(32)]
        [Comment("删除人Id")]
        public virtual string DeleterUserId { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [Comment("删除时间")]
        public virtual DateTime? DeletionTime { get; set; }

        /// <summary>
        /// 删除人名称
        /// </summary>
        [Comment("删除人名称")]
        public virtual string DeleterUserName { get; set; }
    }
}

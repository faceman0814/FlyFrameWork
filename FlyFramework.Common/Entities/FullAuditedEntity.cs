using System.ComponentModel.DataAnnotations;

namespace FlyFramework.Common.Entities
{
    public class FullAuditedEntity<TPrimaryKey> : AuditedEntity<TPrimaryKey>, IFullAuditedEntity<TPrimaryKey>
    {
        public virtual bool IsDeleted { get; set; }

        [MaxLength(32)]
        public virtual string DeleterUserId { get; set; }

        public virtual DateTime? DeletionTime { get; set; }
        public virtual string DeleterUserName { get; set; }
    }
}

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
        public virtual DateTime? LastModificationTime { get; set; }
        public virtual string LastModifierUserName { get; set; }
        [MaxLength(32)]
        public virtual TPrimaryKey LastModifierUserId { get; set; }
    }
}

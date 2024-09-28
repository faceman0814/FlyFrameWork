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
        public virtual DateTime CreationTime { get; set; }
        public virtual string CreatorUserName { get; set; }
        [MaxLength(32)]
        public virtual TPrimaryKey CreatorUserId { get; set; }

        protected CreationAuditedEntity()
        {
            CreationTime = DateTime.Now;
        }
    }
}

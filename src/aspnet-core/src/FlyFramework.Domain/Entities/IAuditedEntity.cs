using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Entities
{
    public interface IAuditedEntity<TPrimaryKey> : ICreationAuditedEntity<TPrimaryKey>
    {
        public new DateTime? LastModificationTime { get; set; }
        public string LastModifierUserName { get; set; }
        public TPrimaryKey LastModifierUserId { get; set; }
    }

}

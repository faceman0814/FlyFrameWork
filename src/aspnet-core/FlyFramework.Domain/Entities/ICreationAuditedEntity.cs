using FlyFramework.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Domain.Entities
{
    public interface ICreationAuditedEntity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        public DateTime CreationTime { get; set; }
        public string CreatorUserName { get; set; }
        public TPrimaryKey CreatorUserId { get; set; }
    }
}

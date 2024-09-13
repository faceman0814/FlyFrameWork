using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Entities
{
    public interface ICreationAuditedEntity<TPrimaryKey>
    {
        public DateTime CreationTime { get; set; }
        public string CreatorUserName { get; set; }
        public TPrimaryKey CreatorUserId { get; set; }
    }
}

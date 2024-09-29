using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Entities
{
    public interface IFullAuditedEntity<TPrimaryKey> : IAuditedEntity<TPrimaryKey>, ISoftDelete
    {
        public string DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeleterUserName { get; set; }
    }

}

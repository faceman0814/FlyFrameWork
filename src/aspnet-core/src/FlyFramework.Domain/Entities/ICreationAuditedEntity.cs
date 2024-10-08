using System;

namespace FlyFramework.Entities
{
    public interface ICreationAuditedEntity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        public DateTime CreationTime { get; set; }
        public string CreatorUserName { get; set; }
        public TPrimaryKey CreatorUserId { get; set; }
    }
}

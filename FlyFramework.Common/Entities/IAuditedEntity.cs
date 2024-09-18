﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Entities
{
    public interface IAuditedEntity<TPrimaryKey> : ICreationAuditedEntity<TPrimaryKey>
    {
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUserName { get; set; }
        public TPrimaryKey LastModifierUserId { get; set; }
    }
}

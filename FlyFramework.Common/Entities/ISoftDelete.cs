using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Entities
{
    public interface ISoftDelete
    {
        public bool IsDeleted { get; set; }
    }
}

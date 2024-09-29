using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Utilities.Redis
{
    public class RedisOptionsConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public int MaxWritePoolSize { get; set; }
        public int MaxReadPoolSize { get; set; }
        public bool AutoStart { get; set; }
        public int LocalCacheTime { get; set; }
        public bool RecordeLog { get; set; }
        public string PreName { get; set; }
        public bool Enable { get; set; }
        public bool SSL { get; set; }
        public int Db { get; set; }
    }
}

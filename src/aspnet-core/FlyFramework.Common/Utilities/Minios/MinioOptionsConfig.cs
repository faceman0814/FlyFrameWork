using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Utilities.Minios
{
    public class MinioOptionsConfig
    {
        public string EndPoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public bool Enable { get; set; }
        public bool Secure { get; set; }
    }
}

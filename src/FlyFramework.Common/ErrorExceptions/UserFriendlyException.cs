using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.ErrorExceptions
{
    public class UserFriendlyException : Exception
    {

        public UserFriendlyException(string message)
            : base(message)
        {
        }
    }
}

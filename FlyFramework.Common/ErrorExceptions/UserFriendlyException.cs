using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.ErrorExceptions
{
    public class UserFriendlyException : Exception
    {
        public string UserMessage { get; private set; }

        public UserFriendlyException(string message, string userMessage)
            : base(message)
        {
            UserMessage = userMessage;
        }
    }
}

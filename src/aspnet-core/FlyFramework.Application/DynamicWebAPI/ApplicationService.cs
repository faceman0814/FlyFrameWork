using FlyFramework.Common.Enums;
using FlyFramework.Common.ErrorExceptions;

using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Application.DynamicWebAPI
{
    public class ApplicationService
    {
        //
        // 摘要:
        //     名称重复错误 传入多语言
        protected void RepetError(string name, string str = "RepetError")
        {
            throw new UserFriendlyException("Error");
        }

        //
        // 摘要:
        //     数据为空 传入多语言
        protected void NullError(string str = "NullError")
        {
            throw new UserFriendlyException("Error");
        }

        protected void ThrowException(string msg, ErrorTypes errorType = ErrorTypes.Exception, string title = "Error")
        {
            switch (errorType)
            {
                case ErrorTypes.Exception:
                    throw new Exception(msg);
                case ErrorTypes.UserFriendlyException:
                    if (title == "Error")
                    {
                        throw new UserFriendlyException(msg);
                    }

                    throw new UserFriendlyException(title + msg);
                default:
                    throw new Exception("Error500Desc");
            }
        }

        protected void ThrowDeleteDefaultVersion(string rdoModuleName)
        {
            throw new UserFriendlyException("Error");
        }

        protected void ThrowUserFriendlyError(string reason)
        {
            throw new UserFriendlyException("Error");
        }

        protected void ThrowDeleteError(string def, string defRef1, string defRef2)
        {
            throw new UserFriendlyException("Error");
        }
    }
}

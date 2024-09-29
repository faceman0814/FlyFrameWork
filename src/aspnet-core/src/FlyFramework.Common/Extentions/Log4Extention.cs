﻿using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Extentions
{
    public class Log4Extention
    {
        public static void InitLog4(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddFilter("System", LogLevel.Warning);
            //loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);//过滤掉系统默认的一些日志
            loggingBuilder.AddLog4Net(new Log4NetProviderOptions()
            {
                Log4NetConfigFileName = "Config/log4net.config",
                Watch = true
            });
        }
    }
}

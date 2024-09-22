using RabbitMQ.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Common.Utilities.RabbitMqs
{
    public interface IRabbitMqManager
    {
        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        IConnection GetConnection();
        /// <summary>
        /// 获取通道
        /// </summary>
        /// <returns></returns>
        IModel GetChannel();
    }
}

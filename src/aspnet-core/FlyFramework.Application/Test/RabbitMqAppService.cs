using FlyFramework.Application.DynamicWebAPI;
using FlyFramework.Common.Utilities.RabbitMqs;

using RabbitMQ.Client.Events;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Application.Test
{
    public class RabbitMqAppService : ApplicationService, IApplicationService
    {
        private readonly IRabbitMqManager _rabbitMqManager;

        public RabbitMqAppService(IServiceProvider serviceProvider, IRabbitMqManager rabbitMqManager) : base(serviceProvider)
        {
            _rabbitMqManager = rabbitMqManager;
        }

        public void Publish()
        {
            //创建一个AMQP 0-9-1频道,该对象提供了大部分 的操作(方法)协议。
            using (var channel = _rabbitMqManager.GetChannel())
            {
                channel.QueueDeclare("RabbitMQ.Test", false, false, false, null);//创建一个名称消息队列
                string message = "Hello RabbitMQ!"; //传递的消息内容
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("", "RabbitMQ.Test", true, null, body); //开始传递
            }
        }

        public void Subscribe()
        {
            using (var channel = _rabbitMqManager.GetChannel())
            {
                channel.QueueDeclare("RabbitMQ.Test", false, false, false, null);//声明一个名称消息队列
                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);//创建一个消费者实现c#事件处理程序实例
                channel.BasicConsume("RabbitMQ.Test", true, null, false, false, null, consumer);//开始消费
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine("已接收： {0}", message);
                };
                Console.ReadLine();
            }
        }
    }
}

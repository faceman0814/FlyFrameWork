using DotNetCore.CAP;

using FlyFramework.Application.DynamicWebAPI;
using FlyFramework.Common.Utilities.EventBus.Distributed;
using FlyFramework.Common.Utilities.EventBus.Local;
using FlyFramework.Common.Utilities.Minios;

using Hangfire;

using Microsoft.AspNetCore.Mvc;

namespace FlyFramework.Application.Test
{
    public class HangFireAppService : ApplicationService, IApplicationService
    {
        public HangFireAppService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost]
        public void HangFireTest()
        {
            #region Hangfire延时执行作业

            BackgroundJob.Schedule<IMessageService>(x => x.SendMessage("Delayed Message"), TimeSpan.FromMinutes(5));

            #endregion Hangfire延时执行作业

            #region Hangfire周期性作业

            RecurringJob.AddOrUpdate<IMessageService>("sendMessageJob", x => x.SendMessage("Hello Message"), "0 2 * * *");

            #endregion Hangfire周期性作业

            var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget!"));

            BackgroundJob.Schedule(() => Console.WriteLine("Delayed!"), TimeSpan.FromDays(7));

            RecurringJob.AddOrUpdate(() => Console.WriteLine("Recurring!"), Cron.Daily);

            BackgroundJob.ContinueWith(jobId, () => Console.WriteLine("Continuation!"));

            //var batchId = BatchJob.StartNew(x =>
            //{
            //    x.Enqueue(() => Console.WriteLine("Job 1"));
            //    x.Enqueue(() => Console.WriteLine("Job 2"));
            //});

            //BatchJob.ContinueWith(batchId, x =>
            //{
            //    x.Enqueue(() => Console.WriteLine("Last Job"));
            //});

            //文档：https://www.bookstack.cn/read/Hangfire-zh-official/3.md
        }

    }

    public interface IMessageService
    {
        void SendMessage(string message);

        void ReceivedMessage(string message);
    }

    public class MessageService : IMessageService
    {
        public void ReceivedMessage(string message)
        {
            Console.WriteLine($"接收消息{message}");
        }

        public void SendMessage(string message)
        {
            Console.WriteLine($"发送消息{message}");
        }
    }
}

using FlyFramework.Common.Attributes;
using FlyFramework.Common.Dependencys;
using FlyFramework.Common.Utilities.EventBus;
using FlyFramework.Common.Utilities.EventBus.Distributed;
using FlyFramework.Common.Utilities.EventBus.Local;
using FlyFramework.Common.Utilities.Minios;
using FlyFramework.WebHost.Models;

using Hangfire;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Minio.DataModel;
using Minio.DataModel.Result;

using MongoDB.Bson.Serialization.IdGenerators;

using ServiceStack.Messaging;

namespace FlyFramework.WebHost.Controllers
{
    [ApiController]
    [DisabledUnitOfWork(true)]
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class TestController : Controller
    {
        private readonly IMinioManager _minioManager;
        private readonly ILocalEventBus _localEventBus;
        private readonly IDistributedEventBus _distributedEventBus;

        public TestController(IMinioManager minioManager, ILocalEventBus localEventBus, IDistributedEventBus distributedEventBus)
        {
            _minioManager = minioManager;
            _localEventBus = localEventBus;
            _distributedEventBus = distributedEventBus;
        }

        /// <summary>
        /// 1、判断指定bucket是否存在
        /// </summary>
        /// <param name="bucketName">bucket 名称</param>
        /// <returns></returns>
        [HttpPost]
        public async Task IsBucketExit()
        {
            await _minioManager.IsExistStr();
        }

        /// <summary>
        /// 2、 创建bucket
        /// </summary>
        /// <param name="bucketName">bucket 名称</param>
        /// <returns></returns>
        [HttpPost]
        public async Task CreateBucket()
        {
            await _minioManager.CreateBucketAsync();
        }

        /// <summary>
        /// 3、移除bucket
        /// </summary>
        /// <param name="bucketName">bucket 名称</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task DeleteBucket()
        {
            await _minioManager.DeleteBucketAsync();
        }

        /// <summary>
        /// 4、获取bucket列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ListAllMyBucketsResult> GetBucketList()
        {
            return await _minioManager.GetList();
        }
        /// <summary>
        /// 1、下载 bucket中的文件
        /// </summary>
        /// <remarks>
        /// 会保存在 D:\\minio-download-files 文件夹内；
        /// 若本地D盘中没有该文件夹，则会自动创建；
        /// </remarks>
        /// <param name="objectName">文件名</param>
        /// <param name="bucketName">桶名，默认flyframework</param>
        [HttpPost]
        public async Task DownloadObject(string objectName)
        {
            await _minioManager.DownloadFile(objectName, "D:\\minio-download-files\\");
        }

        ///<summary>
        /// 2、上传 本地指定文件
        /// </summary>
        /// <remarks>
        /// 上传同名文件，会覆盖之前的
        /// </remarks>
        /// <param name="fileFullPath">上传文件的完整绝对路径，例如：D:\test\test.txt</param>
        /// <param name="bucketName">桶名，默认flyframework</param>
        [HttpPost]
        public async Task UploadObject(string fileFullPath)
        {
            await _minioManager.UploadFile(fileFullPath);
        }

        /// <summary>
        /// 3、删除 指定桶中的指定文件
        /// </summary>
        /// <param name="objectName">文件名</param>
        /// <param name="bucketName">桶名，默认flyframework</param>
        [HttpDelete]
        public async Task DeleteObject(string objectName)
        {
            await _minioManager.DeleteFile(objectName);
        }

        /// <summary>
        /// 4、获取 指定文件的Url链接 （有效期 7天）
        /// </summary>
        /// <remarks>
        /// 只能是已经存在于minio中的任意文件
        /// </remarks>
        /// <param name="objectName">文件名</param>
        /// <param name="bucketName">桶名，默认flyframework</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> GetObjectUrl(string objectName)
        {
            return await _minioManager.GetFileUrl(objectName);
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
        [HttpGet("Local")]
        public async Task Local()
        {
            await _localEventBus.PublishAsync(new TestEventData { TestStr = Guid.NewGuid().ToString() });
        }

        [HttpGet("Distributed")]
        public async Task Distributed()
        {
            await _distributedEventBus.PublishAsync(new TestEventData { TestStr = Guid.NewGuid().ToString() });
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

    [EventName("Test")]
    public class TestEventData : INotification
    {
        public string TestStr { get; set; }
    }

    public class TestEventDataLocalEventHandler : ILocalEventHandler<TestEventData>
    {
        private readonly ILogger<TestEventDataLocalEventHandler> _logger;

        public TestEventDataLocalEventHandler(ILogger<TestEventDataLocalEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TestEventData eventData, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"TestEventDataLocalEventHandler: {eventData.TestStr}");
            return Task.CompletedTask;
        }
    }

    public class TestEventDataDistributedEventHandler : IDistributedEventHandler<TestEventData>, ITransientDependency
    {
        private readonly ILogger<TestEventDataDistributedEventHandler> _logger;

        public TestEventDataDistributedEventHandler(ILogger<TestEventDataDistributedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TestEventData eventData, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"TestEventDataDistributedEventHandler: {eventData.TestStr}");
            return Task.CompletedTask;
        }
    }
}

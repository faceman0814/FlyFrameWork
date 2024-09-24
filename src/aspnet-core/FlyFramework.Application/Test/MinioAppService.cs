using FlyFramework.Common.Utilities.Minios;
using FlyFramework.Domain.ApplicationServices;

using Microsoft.AspNetCore.Mvc;

using Minio.DataModel.Result;

namespace FlyFramework.Application.Test
{
    public class MinioAppService : ApplicationService, IApplicationService
    {
        private readonly IMinioManager _minioManager;

        public MinioAppService(IServiceProvider serviceProvider, IMinioManager minioManager) : base(serviceProvider)
        {
            _minioManager = minioManager;
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
    }
}

using Microsoft.Extensions.Configuration;

using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Result;

using System;
using System.IO;
using System.Threading.Tasks;

namespace FlyFramework.Utilities.Minios
{
    public class MinioManager : IMinioManager
    {
        private readonly IMinioClient _minioClient;
        private readonly string bucketName;

        public MinioManager(IMinioClient minioClient, IConfiguration configuration)
        {
            _minioClient = minioClient;
            bucketName = configuration["Minio:BucketName"];

        }

        public async Task CreateBucketAsync()
        {
            BucketExistsArgs args = new BucketExistsArgs().WithBucket(bucketName);

            bool found = await _minioClient.BucketExistsAsync(args).ConfigureAwait(false);

            if (found)
            {
                throw new Exception($"{bucketName}桶已存在");
            }
            else
            {
                MakeBucketArgs makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs);
            }
        }

        public async Task DeleteBucketAsync()
        {
            BucketExistsArgs args = new BucketExistsArgs().WithBucket(bucketName);

            bool found = await _minioClient.BucketExistsAsync(args).ConfigureAwait(false);

            if (!found)
            {
                throw new Exception($"{bucketName}桶不存在");
            }
            else
            {
                RemoveBucketArgs removeBucketArgs = new RemoveBucketArgs().WithBucket(bucketName);

                await _minioClient.RemoveBucketAsync(removeBucketArgs);
            }

        }

        public async Task DeleteFile(string objectName)
        {
            // 判断bucket是否存在
            bool isExit = await IsExistStr();

            if (!isExit)
            {
                throw new Exception($"{bucketName}桶不存在，文件删除失败");
            }

            RemoveObjectArgs removeObjectArgs = new RemoveObjectArgs()
                            .WithBucket(bucketName)
                            .WithObject(objectName);
            await _minioClient.RemoveObjectAsync(removeObjectArgs);
        }

        public async Task DownloadFile(string objectName, string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
                directoryInfo.Create();
            }

            StatObjectArgs statObjectArgs = new StatObjectArgs()
                                                .WithBucket(bucketName)
                                                .WithObject(objectName);
            await _minioClient.StatObjectAsync(statObjectArgs);

            GetObjectArgs getObjectArgs = new GetObjectArgs()
                                              .WithBucket(bucketName)
                                              .WithObject(objectName)
                                              .WithFile(folderPath + objectName);
            await _minioClient.GetObjectAsync(getObjectArgs);

        }

        public async Task<string> GetFileUrl(string objectName, int expiresIn = 7)
        {
            PresignedGetObjectArgs args = new PresignedGetObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithExpiry(60 * 60 * 24 * expiresIn);
            return await _minioClient.PresignedGetObjectAsync(args);

        }

        public async Task<ListAllMyBucketsResult> GetList()
        {
            return await _minioClient.ListBucketsAsync();
        }

        public async Task<bool> IsExistStr()
        {

            BucketExistsArgs args = new BucketExistsArgs().WithBucket(bucketName);
            return await _minioClient.BucketExistsAsync(args).ConfigureAwait(false);
        }

        public async Task UploadFile(string fileFullPath)
        {
            // 判断bucket是否存在
            bool isExit = await IsExistStr();

            if (!isExit)
            {
                throw new Exception($"{bucketName}桶不存在，文件上传失败");
            }

            string objectName = fileFullPath.Split("\\")[^1];

            // 上传文件
            PutObjectArgs putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithFileName(fileFullPath)
                .WithContentType("application/octet-stream");
            await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
        }
    }
}

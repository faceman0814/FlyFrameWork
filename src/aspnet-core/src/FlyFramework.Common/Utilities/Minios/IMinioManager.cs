using FlyFramework.Dependencys;

using Minio.DataModel.Result;

using System.Threading.Tasks;
namespace FlyFramework.Utilities.Minios
{
    public interface IMinioManager : ISingletonDependency
    {
        /// <summary>
        /// 判断指定bucket是否存在
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        Task<bool> IsExistStr();
        /// <summary>
        /// 创建bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        Task CreateBucketAsync();
        /// <summary>
        /// 删除bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        Task DeleteBucketAsync();
        /// <summary>
        /// 获取所有bucket
        /// </summary>
        /// <returns></returns>
        Task<ListAllMyBucketsResult> GetList();

        /// <summary>
        /// 下载文件 到本地
        /// </summary>
        /// <param name="bucketName">桶名</param>
        /// <param name="objectName">需要下载的文件名</param>
        /// <param name="folderPath">要下载到的文件夹路径</param>
        /// <returns></returns>
        Task DownloadFile(string objectName, string folderPath);

        /// <summary>
        /// 上传文件 指定文件
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileFullPath"></param>
        /// <returns></returns>
        Task UploadFile(string fileFullPath);
        /// <summary>
        /// 删除 指定文件
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        Task DeleteFile(string objectName);
        /// <summary>
        /// 获取指定文件的url链接
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        Task<string> GetFileUrl(string objectName, int expiresIn = 7);
    }
}

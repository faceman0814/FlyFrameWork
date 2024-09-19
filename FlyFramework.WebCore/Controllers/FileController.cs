using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.WebCore.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FileController : Controller
    {
        /// <summary>
        /// 文件控制器
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetFile()
        {
            return "Hello World!";
        }
    }
}

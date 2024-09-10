using Microsoft.AspNetCore.Mvc;

namespace FlyFramework.WebHost.Controllers
{
    [ApiController]
    public class TestAPPController : Controller
    {
        /// <summary>
        /// 控制器API测试11
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/test")]
        public string Test()
        {
            return "Hello World!";
        }
    }
}

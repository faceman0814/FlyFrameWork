using Microsoft.AspNetCore.Mvc;

namespace FlyFramework.WebHost.Controllers
{
    //[SkipActionFilter]
    //[DisabledUnitOfWork(true)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Privacy Policy
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult info()
        {
            return View();
        }

        public IActionResult Index()
        {
            //if (AppSettings.Environment().IsDevelopment())
            //{
            //    ViewBag.Url = "http://localhost:5155";
            //}
            //else
            //{
            //    ViewBag.Url = "http://123.56.30.198:5155";
            //}

            _logger.LogInformation("���ڼ�����ҳ......");
            return View();
        }
    }
}

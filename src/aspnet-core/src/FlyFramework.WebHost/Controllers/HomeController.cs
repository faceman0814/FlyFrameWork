using FlyFramework.Common.Attributes;

using Microsoft.AspNetCore.Mvc;

namespace FlyFramework.WebHost.Controllers
{
    [SkipActionFilter]
    [DisabledUnitOfWork(true)]
    public class HomeController : Controller
    {
        // GET: HomeController
        public ActionResult Index()
        {
            return View();
        }
    }
}

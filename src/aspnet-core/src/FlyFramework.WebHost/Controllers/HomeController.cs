using FlyFramework.Attributes;

using Microsoft.AspNetCore.Mvc;

namespace FlyFramework.Controllers
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

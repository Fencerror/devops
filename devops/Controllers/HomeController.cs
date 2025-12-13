using Microsoft.AspNetCore.Mvc;

namespace DevOpsPollApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

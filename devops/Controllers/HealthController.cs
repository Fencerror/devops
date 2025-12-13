using Microsoft.AspNetCore.Mvc;

namespace DevOpsPollApp.Controllers
{
    public class HealthController : Controller
    {
        [HttpGet("/health")]
        public IActionResult Index()
        {
            return Ok("OK");
        }
    }
}

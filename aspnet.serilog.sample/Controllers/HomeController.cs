using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace aspnet.serilog.sample.Controllers
{
    public class HomeController : Controller
    {
        ILogger<HomeController> logger;

        // Injectamos el logger en el constructor
        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        public IActionResult Index()
        {
            this.logger.LogDebug("Index was called");
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}

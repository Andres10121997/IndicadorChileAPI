using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> Logger;
        


        public HomeController(ILogger<HomeController> Logger)
            : base()
        {
            this.Logger = Logger;
        }



        public IActionResult Index()
        {
            return View();
        }
    }
}
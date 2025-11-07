using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Website.Controllers
{
    public class SIIController : Controller
    {
        private readonly ILogger<SIIController> Logger;



        public SIIController(ILogger<SIIController> Logger)
            : base()
        {
            this.Logger = Logger;
        }



        public IActionResult UF()
        {
            return View();
        }
    }
}
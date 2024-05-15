using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ResourceAllocationTool.Controllers
{

    [Authorize]
    public class PeriodsController : Controller
    {


        public PeriodsController()
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

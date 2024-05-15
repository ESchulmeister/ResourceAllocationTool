using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ResourceAllocationTool.Controllers
{

    [Authorize]
    public class EmpByManagerController : Controller
    {


        public EmpByManagerController()
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

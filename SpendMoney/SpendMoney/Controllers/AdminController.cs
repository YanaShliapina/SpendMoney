using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SpendMoney.Controllers
{
    [Authorize]
    [Route("Admin")]
    public class AdminController : Controller
    {
        [Route("Index")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}

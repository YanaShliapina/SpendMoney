using Microsoft.AspNetCore.Mvc;

namespace SpendMoney.Controllers
{
    public class UserDreamController : Controller
    {
        [HttpGet]
        [Route("CreateDream")]
        public IActionResult CreateDream()
        {
            return View();
        }
    }
}

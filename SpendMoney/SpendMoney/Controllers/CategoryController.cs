using Microsoft.AspNetCore.Mvc;
using SpendMoney.ViewModels;

namespace SpendMoney.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("GetUserCategories")]
        [HttpGet]
        public IActionResult GetUserCategories()
        {
            CategoryViewModel s = new CategoryViewModel();
            return View(s);
        }
    }
}

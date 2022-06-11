using Microsoft.AspNetCore.Mvc;

namespace SpendMoney.Controllers
{
    public class StatisticController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

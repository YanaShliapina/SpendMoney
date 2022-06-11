using Microsoft.AspNetCore.Mvc;

namespace SpendMoney.Controllers
{
    public class TransactionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

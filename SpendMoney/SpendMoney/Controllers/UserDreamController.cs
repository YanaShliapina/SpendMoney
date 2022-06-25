using Microsoft.AspNetCore.Mvc;
using SpendMoney.ViewModels;

namespace SpendMoney.Controllers
{
    [Route("[controller]")]
    public class UserDreamController : Controller
    {
        [HttpGet]
        [Route("CreateDream")]
        public IActionResult CreateDream()
        {
            return View();
        }

        [HttpPost]
        [Route("CreateDream")]
        public IActionResult CreateDream(CreateDreamViewModel request)
        {
            return View();
        }

        [HttpGet]
        [Route("Calculate")]
        public JsonResult Calculate(decimal targetAmount, decimal currentAmount, DateTime endDate, int saveType)
        {
            var needAmount = targetAmount - currentAmount;
            var dayCount = (endDate.Date - DateTime.Now.Date).TotalDays;

            string result = string.Empty;

            if(saveType == 1)
            {
                var weekCount = Math.Round(((decimal)dayCount) / 7, 1);
                var amount = needAmount / weekCount;

                result = "Кількість тижнів: " + weekCount + "<br/>" + "Сума кожного платежу: " + amount;
            }
            else if(saveType == 2)
            {
                var monthCount = Math.Round(((decimal)dayCount) / 30, 1);
                var amount = Math.Round(needAmount / monthCount, 2);

                result = "Кількість місяців: " + monthCount + "<br/>" + "Сума кожного платежу: " + amount;
            }
            else if(saveType == 3)
            {
                var amount = Math.Round(needAmount / (decimal)dayCount, 2);

                result = "Кількість днів: " + dayCount + "<br/>" + "Сума кожного платежу: " + amount;
            }

            return new JsonResult(result);
        }
    }
}

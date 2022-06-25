using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SpendMoney.Core.Models;
using SpendMoney.Core.Services.Interfaces;
using SpendMoney.ViewModels;

namespace SpendMoney.Controllers
{
    [Route("[controller]")]
    public class UserDreamController : Controller
    {
        private readonly IUserDreamService _userDreamService;
        private readonly IAccountServicecs _accountService;
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        public UserDreamController(IMapper mapper, IUserDreamService userDreamService, IAccountServicecs accountService, ITransactionService transactionService)
        {
            _mapper = mapper;
            _userDreamService = userDreamService;
            _accountService = accountService;
            _transactionService = transactionService;
        }

        [HttpGet]
        [Route("CreateDream")]
        public IActionResult CreateDream()
        {
            return View();
        }

        [HttpPost]
        [Route("CreateDream")]
        public async Task<IActionResult> CreateDream(CreateDreamViewModel request)
        {
            var user = await _accountService.GetCurrentUser(User);
            var createRQ =_mapper.Map<CreateDreamRQ>(request);
            createRQ.UserId = user.Id;

            await _userDreamService.CreateDream(createRQ);

            return RedirectToAction("Index", "Home");
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
                var amount = Math.Round(needAmount / weekCount);

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

        [Route("ChargeForDream")]
        [HttpGet]
        public async Task<IActionResult> ChargeForDream(int dreamId)
        {
            var user = await _accountService.GetCurrentUser(User);
            var accounts = await _accountService.GetAccountsByUserId(user.Id);
            var dream = await _userDreamService.GetDreamId(dreamId);

            var viewModel = new ChargeForDreamViewModel
            {
                DreamId = dreamId,
                UserAccounts = accounts,
                Amount = dream.EachPayAmount,
                DreamName = dream.Name
            };

            return View(viewModel);
        }

        [Route("ChargeForDream")]
        [HttpPost]
        public async Task<IActionResult> ChargeForDream(ChargeForDreamViewModel request)
        {
            ModelState.Remove("UserAccounts");

            var user = await _accountService.GetCurrentUser(User);

            if (!ModelState.IsValid)
            {
                var accounts = await _accountService.GetAccountsByUserId(user.Id);

                request.UserAccounts = accounts;

                return View(request);
            }

            await _transactionService.CreateTransaction(new CreateTransactionRQ
            {
                AccountId = request.SelectedAccountId,
                Amount = request.Amount,
                Description = "Перерахування на мрію " + request.DreamName,
                TransactionType = TransactionTypes.Transfer,
                UserDreamId = request.DreamId,
                UserId = user.Id,
            });

            return RedirectToAction("Index", "Home");
        }
    }
}

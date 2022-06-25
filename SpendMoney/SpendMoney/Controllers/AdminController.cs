using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendMoney.Core.Services.Interfaces;
using SpendMoney.ViewModels;

namespace SpendMoney.Controllers
{
    [Authorize]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IAccountServicecs _accountService;
        private readonly ITransactionService _transactionService;

        public AdminController(ICategoryService categoryService, IAccountServicecs accountService, ITransactionService transactionService)
        {
            _categoryService = categoryService;
            _accountService = accountService;
            _transactionService = transactionService;
        }

        [Route("Index")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var spendTrans = await _transactionService.GetTransactionsByType(TransactionTypes.Spend);
            var addTrans = await _transactionService.GetTransactionsByType(TransactionTypes.Add);

            var viewModel = new AdminPanelViewModel
            {
                CategoryList = await _categoryService.GetAllCategoryList(),
                AddAmount = Math.Round(addTrans.Sum(x => x.Amount) / addTrans.Count()),
                SpendAmount = Math.Round(spendTrans.Sum(x => x.Amount) / spendTrans.Count()),
            };

            return View(viewModel);
        }
    }
}

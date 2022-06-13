using Microsoft.AspNetCore.Mvc;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Models;
using SpendMoney.Core.Services.Interfaces;
using SpendMoney.ViewModels;

namespace SpendMoney.Controllers
{
    [Route("[controller]")]
    public class TransactionController : Controller
    {
        private readonly IAccountServicecs _accountService;
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;

        public TransactionController(IAccountServicecs accountService, ITransactionService transactionService, ICategoryService categoryService)
        {
            _accountService = accountService;
            _transactionService = transactionService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("AddMoneyTransactionToAccount")]
        public async Task<IActionResult> AddMoneyTransactionToAccount(int accountId)
        {
            var account = await _accountService.GetAccountById(accountId);

            var viewModel = new AddMoneyTransactionToAccountViewModel
            {
                Account = account
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("AddMoneyTransactionToAccount")]
        public async Task<IActionResult> AddMoneyTransactionToAccount(AddMoneyTransactionToAccountViewModel viewModel)
        {
            var user = await _accountService.GetCurrentUser(User);
            await _accountService.AddAmount(viewModel.Account.AccountId, viewModel.AmountToAdd);
            await _transactionService.CreateTransaction(new CreateTransactionRQ
            {
                Amount = viewModel.AmountToAdd,
                AccountId = viewModel.Account.AccountId,
                Description = viewModel.Description,
                UserId = user.Id,
                TransactionType = TransactionTypes.Add
            });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("RemoveMoneyTransactionFromAccount")]
        public async Task<IActionResult> RemoveMoneyTransactionFromAccount(int accountId)
        {
            var user = await _accountService.GetCurrentUser(User);

            var viewModel = new RemoveMoneyTransactionFromAccountViewModel
            {
               Account = await _accountService.GetAccountById(accountId),
               Categories = await _categoryService.GetCategoryListByUserId(user.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("RemoveMoneyTransactionFromAccount")]
        public async Task<IActionResult> RemoveMoneyTransactionFromAccount(RemoveMoneyTransactionFromAccountViewModel viewModel)
        {
            var user = await _accountService.GetCurrentUser(User);
            await _transactionService.CreateTransaction(new CreateTransactionRQ
            {
                AccountId = viewModel.Account.AccountId,
                Description = viewModel.Description,
                UserId = user.Id,
                TransactionType = TransactionTypes.Spend,
                CategoryId = viewModel.CategoryId,
                Amount = viewModel.Amount
            });

            await _accountService.RemoveAmount(viewModel.Account.AccountId, viewModel.Amount);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("TransferToAccount")]
        public async Task<IActionResult> TransferToAccount(int accountId)
        {
            var user = await _accountService.GetCurrentUser(User);
            var account = await _accountService.GetAccountById(accountId);
            var foundAccounts = await _accountService.GetAccountsByUserId(user.Id);

            var viewModel = new TransferToAccountViewModel
            {
                Account = account,
                UserAccounts = foundAccounts.Where(x => x.AccountId != account.AccountId).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("TransferToAccount")]
        public async Task<IActionResult> TransferToAccount(TransferToAccountViewModel viewModel)
        {
            var user = await _accountService.GetCurrentUser(User);

            var transactionId = await _transactionService.CreateTransaction(new CreateTransactionRQ
            {
                AccountId = viewModel.Account.AccountId,
                Description = viewModel.Description,
                UserId = user.Id,
                TransactionType = TransactionTypes.Transfer,
                Amount = viewModel.Amount,
                TransferToAccountId = viewModel.ToAccountId
            });

            await _accountService.RemoveAmount(viewModel.Account.AccountId, viewModel.Amount);

            var transaction = await _transactionService.GetTransactionById(transactionId);
            await _accountService.AddAmount(viewModel.ToAccountId, transaction.Amount);

            return RedirectToAction("Index", "Home");
        }
    }
}

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

        [HttpGet]
        [Route("AddMoneyToCategory")]
        public async Task<IActionResult> AddMoneyToCategory(int categoryId)
        {
            var user = await _accountService.GetCurrentUser(User);
            var category = await _categoryService.GetCategoryById(categoryId);
            var userAccounts = await _accountService.GetAccountsByUserId(user.Id);

            var viewModel = new AddMoneyToCategoryViewModel
            {
                Category = category,
                UserAccounts = userAccounts
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("AddMoneyToCategory")]
        public async Task<IActionResult> AddMoneyToCategory(AddMoneyToCategoryViewModel viewModel)
        {
            ModelState.Remove("UserAccounts");
            ModelState.Remove("Category.Name");
            ModelState.Remove("Category.Color");
            ModelState.Remove("Category.Image");
            ModelState.Remove("Category.Description");

            var user = await _accountService.GetCurrentUser(User);

            if (ModelState.IsValid == false)
            {
                var category = await _categoryService.GetCategoryById(viewModel.Category.Id);
                var userAccounts = await _accountService.GetAccountsByUserId(user.Id);

                viewModel.Category = category;
                viewModel.UserAccounts = userAccounts;

                return View(viewModel);
            }

            var transactionId = await _transactionService.CreateTransaction(new CreateTransactionRQ
            {
                AccountId = viewModel.AccountId,
                Description = viewModel.Description,
                UserId = user.Id,
                TransactionType = TransactionTypes.Spend,
                Amount = viewModel.Amount,
                CategoryId = viewModel.Category.Id
            });

            await _accountService.RemoveAmount(viewModel.AccountId, viewModel.Amount);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("History")]
        public async Task<IActionResult> History()
        {
            //set accounts and cats
            var userId = (await _accountService.GetCurrentUser(User)).Id;
            var accs = await _accountService.GetAccountsByUserId(userId);
            var cats = await _categoryService.GetCategoryListByUserId(userId);

            var foundTransactions = await _transactionService.GetTransactionByFilter(new TransactionFilter
            {
                UserId = (await _accountService.GetCurrentUser(User)).Id
            });

            var transactionsForChart = foundTransactions.Where(x => x.TransactionType == (int)TransactionTypes.Spend).ToList();
            var allSum = transactionsForChart.Sum(x => x.Amount);
            var categoryNameList = transactionsForChart.Select(x => x.Category.Name.ToString()).Distinct().ToList();

            List<decimal> values = new List<decimal>();
            
            foreach (var catname in categoryNameList)
            {
                var s = transactionsForChart.Where(x => x.Category.Name == catname).ToList().Sum(x => x.Amount);
                values.Add(Math.Round(((s / allSum) * 100), 2));
            }

            var categoryNameListForChart = String.Join(',', categoryNameList);
            var valuesForChart = String.Join(',', values);

            return View(new TransactionHistoryViewModel
            {
                UserAccounts = accs,
                Categories = cats,
                Transactions = foundTransactions,
                CategoryListForChart = categoryNameListForChart,
                CategoryValueListForChart = valuesForChart
            });
        }

        [HttpPost]
        [Route("History")]
        public async Task<IActionResult> History(TransactionHistoryViewModel viewModel)
        {
            List<int> accountIds = new List<int>();
            List<int> categoryIds = new List<int>();
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (!String.IsNullOrEmpty(viewModel.StartDate))
            {
                startDate = DateTime.Parse(viewModel.StartDate);
            }

            if (!String.IsNullOrEmpty(viewModel.EndDate))
            {
                endDate = DateTime.Parse(viewModel.EndDate);
            }

            if (!String.IsNullOrEmpty(viewModel.AccountId))
            {
                if (viewModel.AccountId[0] == ',')
                {
                    viewModel.AccountId = viewModel.AccountId.Remove(0, 1);
                }
                
                viewModel.AccountId.Split(',').ToList().ForEach(x => accountIds.Add(Convert.ToInt32(x)));
            }

            if (!String.IsNullOrEmpty(viewModel.CategoryId))
            {
                if (viewModel.CategoryId[0] == ',')
                {
                    viewModel.CategoryId = viewModel.CategoryId.Remove(0, 1);
                }
                
                viewModel.CategoryId.Split(',').ToList().ForEach(x => categoryIds.Add(Convert.ToInt32(x)));
            }

            var foundTransactions = await _transactionService.GetTransactionByFilter(new TransactionFilter
            {
                AccountIds = accountIds,
                CategoryIds = categoryIds,
                StartDate = startDate,
                EndDate = endDate,
                UserId = (await _accountService.GetCurrentUser(User)).Id
            });

            var userId = (await _accountService.GetCurrentUser(User)).Id;
            var accs = await _accountService.GetAccountsByUserId(userId);
            var cats = await _categoryService.GetCategoryListByUserId(userId);

            var transactionsForChart = foundTransactions.Where(x => x.TransactionType == (int)TransactionTypes.Spend).ToList();
            var allSum = transactionsForChart.Sum(x => x.Amount);
            var categoryNameList = transactionsForChart.Select(x => x.Category.Name.ToString()).Distinct().ToList();

            List<decimal> values = new List<decimal>();

            foreach (var catname in categoryNameList)
            {
                var s = transactionsForChart.Where(x => x.Category.Name == catname).ToList().Sum(x => x.Amount);
                values.Add(Math.Round(((s / allSum) * 100), 2));
            }

            var categoryNameListForChart = String.Join(',', categoryNameList);
            var valuesForChart = String.Join(',', values);

            viewModel.Transactions = foundTransactions;
            viewModel.UserAccounts = accs;
            viewModel.Categories = cats;
            viewModel.CategoryListForChart = categoryNameListForChart;
            viewModel.CategoryValueListForChart = valuesForChart;

            return View(viewModel);
        }

        [HttpGet]
        [Route("ChangeTransaction")]
        public async Task<IActionResult> ChangeTransaction(int id)
        {
            var user = await _accountService.GetCurrentUser(User);
            var transaction = await _transactionService.GetTransactionById(id);

            var viewModel = new ChangeTransactionViewModel
            {
                Transaction = transaction,
                Categories = await _categoryService.GetCategoryListByUserId(user.Id),
                UserAccounts = await _accountService.GetAccountsByUserId(user.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("ChangeTransaction")]
        public async Task<IActionResult> ChangeTransaction(ChangeTransactionViewModel viewModel)
        {
            ModelState.Remove("Categories");
            ModelState.Remove("UserAccounts");
            ModelState.Remove("Transaction.UserAccount");
            ModelState.Remove("Transaction.Category.Name");
            ModelState.Remove("Transaction.Category.Color");
            ModelState.Remove("Transaction.Category.Image");
            ModelState.Remove("Transaction.Category.Description");

            if (!ModelState.IsValid)
            {
                var user = await _accountService.GetCurrentUser(User);
                var transaction = await _transactionService.GetTransactionById(Convert.ToInt32(viewModel.Transaction.TransactionId));

                viewModel.Transaction = transaction;
                viewModel.Categories = await _categoryService.GetCategoryListByUserId(user.Id);
                viewModel.UserAccounts = await _accountService.GetAccountsByUserId(user.Id);

                return View(viewModel);
            }

            await _transactionService.UpdateTransaction(viewModel.Transaction);

            return RedirectToAction("History");
        }

        [HttpGet]
        [Route("RemoveTransaction")]
        public async Task<IActionResult> RemoveTransaction(int id)
        {
            await _transactionService.RemoveTransaction(id);

            return RedirectToAction("History");
        }
    }
}

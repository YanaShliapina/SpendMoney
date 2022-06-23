using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Entities;
using SpendMoney.Core.Models;
using SpendMoney.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;
        private readonly IAccountServicecs _accountService;

        public TransactionService(ApplicationDbContext context, IMapper mapper, ICategoryService categoryService, IAccountServicecs accountService)
        {
            _context = context;
            _mapper = mapper;
            _categoryService = categoryService;
            _accountService = accountService;
        }

        public async Task<int> CreateTransaction(CreateTransactionRQ request)
        {
            var transaction = _mapper.Map<Transaction>(request);
            transaction.Date = DateTime.Now;
            transaction.Type = _context.TransactionTypes.First(x => x.InternalEnumValue == (int)request.TransactionType).Id;

            if (request.CategoryId != null)
            {
                var category = _context.Categories.First(x => x.Id == request.CategoryId);
                var account = _context.UserMoneyAccounts
                    .Include(x => x.Account)
                    .ThenInclude(x => x.Currency)
                    .First(x => x.Id == request.AccountId);

                var isNativeCurrency = account.Account.Currency.ShortName == "UAH";

                if (!isNativeCurrency)
                {
                    var convertRate = await _context.CurrencyExchanges
                                    .Include(x => x.FromNavigation)
                                    .Include(x => x.ToNavigation)
                                    .FirstOrDefaultAsync(x => x.FromNavigation.Id == account.Account.CurrencyId &&
                                        x.ToNavigation.ShortName == "UAH");

                    transaction.Amount = Math.Round((decimal)transaction.Amount * convertRate.Rate, 2);
                }
            }

            if(request.TransferToAccountId != null)
            {
                var account = _context.UserMoneyAccounts
                    .Include(x => x.Account)
                    .ThenInclude(x => x.Currency)
                    .First(x => x.Id == request.AccountId);

                var toAccount = _context.UserMoneyAccounts
                    .Include(x => x.Account)
                    .ThenInclude(x => x.Currency)
                    .First(x => x.Id == request.TransferToAccountId);

                var isNativeCurrency = account.Account.Currency.ShortName == toAccount.Account.Currency.ShortName;

                if (!isNativeCurrency)
                {
                    var convertRate = await _context.CurrencyExchanges
                                    .Include(x => x.FromNavigation)
                                    .Include(x => x.ToNavigation)
                                    .FirstOrDefaultAsync(x => x.FromNavigation.Id == account.Account.CurrencyId &&
                                        x.ToNavigation.ShortName == toAccount.Account.Currency.ShortName);

                    transaction.Amount = Math.Round((decimal)transaction.Amount * convertRate.Rate, 2);
                }
            }

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return transaction.Id;
        }

        public async Task<List<TransactionDto>> GetTransactionByFilter(TransactionFilter filter)
        {
            var query = _context.Transactions
                .Include(x => x.Account)
                .ThenInclude(x => x.Account)
                .Include(x => x.Category)
                .Include(x => x.TypeNavigation)
                .Where(x => x.UserId == filter.UserId);

            if((filter?.AccountIds?.Count ?? 0) > 0)
            {
                query = query.Where(x => filter.AccountIds.Contains(x.AccountId));
            }

            if ((filter?.CategoryIds?.Count ?? 0) > 0)
            {
                query = query.Where(x => filter.CategoryIds.Contains((int)x.CategoryId));
            }

            if(filter.StartDate != null)
            {
                query = query.Where(x => x.Date.Date >= filter.StartDate.Value.Date);
            }

            if (filter.EndDate != null)
            {
                query = query.Where(x => x.Date.Date <= filter.EndDate.Value.Date);
            }

            var result = query.ToList();

            return _mapper.Map<List<TransactionDto>>(result);
        }

        public async Task<TransactionDto> GetTransactionById(int id)
        {
            var foundTransaction = await _context.Transactions
                .Include(x => x.Account)
                .ThenInclude(x => x.Account)
                .Include(x => x.Category)
                .Include(x => x.TypeNavigation)
                .FirstAsync(x => x.Id == id);

            return _mapper.Map<TransactionDto>(foundTransaction);
        }

        public async Task<int> RemoveTransaction(int id)
        {
            var foundTrans = await _context.Transactions
                .Include(x => x.Account)
                .Include(x => x.TypeNavigation)
                .FirstAsync(x => x.Id == id);

            if(foundTrans.TypeNavigation.InternalEnumValue == (int)TransactionTypes.Spend)
            {
                var account = _context.UserMoneyAccounts.First(x => x.Id != foundTrans.AccountId);
                account.Amount += foundTrans.Amount;
            }
            else if(foundTrans.TypeNavigation.InternalEnumValue == (int)TransactionTypes.Add)
            {
                var account = _context.UserMoneyAccounts.First(x => x.Id != foundTrans.AccountId);
                account.Amount -= foundTrans.Amount;
            }
            else if(foundTrans.TypeNavigation.InternalEnumValue == (int)TransactionTypes.Transfer)
            {
                var account = _context.UserMoneyAccounts.First(x => x.Id != foundTrans.AccountId);
                account.Amount += foundTrans.Amount;

                var account2 = _context.UserMoneyAccounts.First(x => x.Id != foundTrans.AccountId);
                account2.Amount -= foundTrans.Amount;
            }

            _context.Transactions.Remove(foundTrans);

            await _context.SaveChangesAsync();

            return id;
        }

        public async Task<TransactionDto> UpdateTransaction(TransactionDto updatedTransaction)
        {
            var tranId = Convert.ToInt32(updatedTransaction.TransactionId);
            var initialTran = await _context.Transactions
                .Include(x => x.Account)
                .ThenInclude(x => x.Account)
                .ThenInclude(x => x.Currency)
                .Include(x => x.Category)
                .Include(x => x.TypeNavigation)
                .FirstAsync(x => x.Id == tranId);

            initialTran.Description = updatedTransaction.Description;
            initialTran.Date = updatedTransaction.Date;
            initialTran.CategoryId = updatedTransaction.Category.Id == 0 ? null : updatedTransaction.Category.Id;

            if(initialTran.AccountId != updatedTransaction.AccountId)
            {
                var newAccount = await _accountService.GetAccountById(updatedTransaction.AccountId);

                if(initialTran.Account.Account.Currency.ShortName != newAccount.CurrencyShortName)
                {
                    if(initialTran.Account.Account.Currency.ShortName == "UAH")
                    {
                        initialTran.Account.Amount += initialTran.Amount;
                    }
                    else
                    {
                        var exchangeRate = await _context.CurrencyExchanges
                            .Include(x => x.FromNavigation)
                            .Include(x => x.ToNavigation)
                            .FirstAsync(x => x.FromNavigation.ShortName == initialTran.Account.Account.Currency.ShortName
                                && x.ToNavigation.ShortName == newAccount.CurrencyShortName);

                        initialTran.Account.Amount += (initialTran.Amount * exchangeRate.Rate);
                    }

                    var currentTargetAccount = _context.UserMoneyAccounts
                        .Include(x => x.Account)
                        .ThenInclude(x => x.Currency)
                        .First(x => x.AccountId == newAccount.AccountId);

                    if (currentTargetAccount.Account.Currency.ShortName == "UAH")
                    {
                        currentTargetAccount.Amount -= updatedTransaction.Amount;
                    }
                    else
                    {
                        var exchangeRate = await _context.CurrencyExchanges
                            .Include(x => x.FromNavigation)
                            .Include(x => x.ToNavigation)
                            .FirstAsync(x => x.FromNavigation.ShortName == "UAH"
                                && x.ToNavigation.ShortName == currentTargetAccount.Account.Currency.ShortName);

                        currentTargetAccount.Amount -= Math.Round((updatedTransaction.Amount * exchangeRate.Rate), 2);
                    }
                }

                initialTran.AccountId = updatedTransaction.AccountId;
            }
            else
            {
                initialTran.Amount = updatedTransaction.Amount;
            }

            if(initialTran.TransferAccountId == null && updatedTransaction.TransferAccountId > 0)
            {
                initialTran.TransferAccountId = updatedTransaction.TransferAccountId;

                var transferTargetAccount = _context.UserMoneyAccounts
                        .Include(x => x.Account)
                        .ThenInclude(x => x.Currency)
                        .First(x => x.AccountId == updatedTransaction.TransferAccountId);

                if (transferTargetAccount.Account.Currency.ShortName == "UAH")
                {
                    transferTargetAccount.Amount += updatedTransaction.Amount;
                }
                else
                {
                    var exchangeRate = await _context.CurrencyExchanges
                        .Include(x => x.FromNavigation)
                        .Include(x => x.ToNavigation)
                        .FirstAsync(x => x.FromNavigation.ShortName == "UAH"
                            && x.ToNavigation.ShortName == transferTargetAccount.Account.Currency.ShortName);

                    transferTargetAccount.Amount += (updatedTransaction.Amount * exchangeRate.Rate);
                }
            }

            if (initialTran.TransferAccountId != null && updatedTransaction.Category.Id > 0)
            {
                var untransferTargetAccount = _context.UserMoneyAccounts
                        .Include(x => x.Account)
                        .ThenInclude(x => x.Currency)
                        .First(x => x.AccountId == updatedTransaction.TransferAccountId);

                if (untransferTargetAccount.Account.Currency.ShortName == "UAH")
                {
                    untransferTargetAccount.Amount -= updatedTransaction.Amount;
                }
                else
                {
                    var exchangeRate = await _context.CurrencyExchanges
                        .Include(x => x.FromNavigation)
                        .Include(x => x.ToNavigation)
                        .FirstAsync(x => x.FromNavigation.ShortName == "UAH"
                            && x.ToNavigation.ShortName == untransferTargetAccount.Account.Currency.ShortName);

                    untransferTargetAccount.Amount -= (updatedTransaction.Amount * exchangeRate.Rate);
                }
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<TransactionDto>(initialTran);
        }
    }
}

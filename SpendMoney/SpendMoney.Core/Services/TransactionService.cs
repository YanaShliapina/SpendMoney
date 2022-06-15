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

        public TransactionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            var foundTransaction = await _context.Transactions.FirstAsync(x => x.Id == id);

            return _mapper.Map<TransactionDto>(foundTransaction);
        }
    }
}

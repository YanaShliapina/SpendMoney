using SpendMoney.Core.DTOs;
using SpendMoney.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<int> CreateTransaction(CreateTransactionRQ request);
        Task<int> RemoveTransaction(int id);
        Task<TransactionDto> GetTransactionById(int id);
        Task<List<TransactionDto>> GetTransactionByFilter(TransactionFilter filter);
        Task<TransactionDto> UpdateTransaction(TransactionDto updatedTransaction);
    }
}

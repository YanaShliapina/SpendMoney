using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum TransactionTypes
{
    Add,
    Spend,
    Transfer
}

namespace SpendMoney.Core.Models
{
    public class CreateTransactionRQ
    {
        public int AccountId { get; set; }
        public TransactionTypes TransactionType { get; set; }
        public int? CategoryId { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public string UserId { get; set; }
        public int? TransferToAccountId { get; set; }
    }
}

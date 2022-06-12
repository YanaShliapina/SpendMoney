using System;
using System.Collections.Generic;

namespace SpendMoney.Core.Entities
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int Type { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int? CategoryId { get; set; }
        public int? TransferAccountId { get; set; }
        public string UserId { get; set; } = null!;

        public virtual UserMoneyAccount Account { get; set; } = null!;
        public virtual Category? Category { get; set; }
        public virtual UserMoneyAccount? TransferAccount { get; set; }
        public virtual TransactionType TypeNavigation { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}

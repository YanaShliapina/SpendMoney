using System;
using System.Collections.Generic;

namespace SpendMoney.Core.Entities
{
    public partial class UserMoneyAccount
    {
        public UserMoneyAccount()
        {
            TransactionAccounts = new HashSet<Transaction>();
            TransactionTransferAccounts = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int AccountId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Limit { get; set; }

        public virtual MoneyAccount Account { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
        public virtual ICollection<Transaction> TransactionAccounts { get; set; }
        public virtual ICollection<Transaction> TransactionTransferAccounts { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SpendMoney.Core.Entities
{
    public partial class TransactionType
    {
        public TransactionType()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}

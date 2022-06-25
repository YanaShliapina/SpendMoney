using System;
using System.Collections.Generic;

namespace SpendMoney.Core.Entities
{
    public partial class UserDream
    {
        public UserDream()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int SaveType { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public string UserId { get; set; } = null!;

        public virtual AspNetUser User { get; set; } = null!;
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}

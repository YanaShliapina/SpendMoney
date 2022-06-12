using System;
using System.Collections.Generic;

namespace SpendMoney.Core.Entities
{
    public partial class Category
    {
        public Category()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string? UserId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Limit { get; set; }
        public string Color { get; set; } = null!;
        public int ImageId { get; set; }

        public virtual Image Image { get; set; } = null!;
        public virtual AspNetUser? User { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}

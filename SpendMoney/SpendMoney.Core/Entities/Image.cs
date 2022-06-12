using System;
using System.Collections.Generic;

namespace SpendMoney.Core.Entities
{
    public partial class Image
    {
        public Image()
        {
            Categories = new HashSet<Category>();
            MoneyAccounts = new HashSet<MoneyAccount>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Desription { get; set; }
        public string? Path { get; set; }
        public int Type { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<MoneyAccount> MoneyAccounts { get; set; }
    }
}

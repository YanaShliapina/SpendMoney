using System;
using System.Collections.Generic;

namespace SpendMoney.Core.Entities
{
    public partial class Currency
    {
        public Currency()
        {
            MoneyAccounts = new HashSet<MoneyAccount>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ShortName { get; set; } = null!;

        public virtual ICollection<MoneyAccount> MoneyAccounts { get; set; }
    }
}

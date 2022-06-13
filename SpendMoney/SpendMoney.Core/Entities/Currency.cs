using System;
using System.Collections.Generic;

namespace SpendMoney.Core.Entities
{
    public partial class Currency
    {
        public Currency()
        {
            CurrencyExchangeFromNavigations = new HashSet<CurrencyExchange>();
            CurrencyExchangeToNavigations = new HashSet<CurrencyExchange>();
            MoneyAccounts = new HashSet<MoneyAccount>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ShortName { get; set; } = null!;

        public virtual ICollection<CurrencyExchange> CurrencyExchangeFromNavigations { get; set; }
        public virtual ICollection<CurrencyExchange> CurrencyExchangeToNavigations { get; set; }
        public virtual ICollection<MoneyAccount> MoneyAccounts { get; set; }
    }
}

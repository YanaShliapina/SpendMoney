using System;
using System.Collections.Generic;

namespace SpendMoney.Core.Entities
{
    public partial class CurrencyExchange
    {
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public decimal Rate { get; set; }

        public virtual Currency FromNavigation { get; set; } = null!;
        public virtual Currency ToNavigation { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;

namespace SpendMoney.Core.Entities
{
    public partial class MoneyAccount
    {
        public MoneyAccount()
        {
            UserMoneyAccounts = new HashSet<UserMoneyAccount>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int ImageId { get; set; }
        public int CurrencyId { get; set; }

        public virtual Currency Currency { get; set; } = null!;
        public virtual Image Image { get; set; } = null!;
        public virtual ICollection<UserMoneyAccount> UserMoneyAccounts { get; set; }
    }
}

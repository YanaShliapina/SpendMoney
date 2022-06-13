using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.Models
{
    public class UpdateUserAccountRQ
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrencyId { get; set; }
        public decimal Amount { get; set; }
        public string Color { get; set; }
        public int ImageId { get; set; }
    }
}

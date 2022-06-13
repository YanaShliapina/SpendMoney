using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.Models
{
    public class CreateUserAccountRQ
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrencyId { get; set; }
        public int ImageId { get; set; }
        public string Amount { get; set; }
        public string Color { get; set; }
        public string UserId { get; set; }
    }
}

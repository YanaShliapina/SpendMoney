using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.Models
{
    public class CreateDreamRQ
    {
        public string Name { get; set; }
        public int SaveType { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public string UserId { get; set; }
    }
}

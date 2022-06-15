using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.Models
{
    public class TransactionFilter
    {
        public string UserId { get; set; }

        public List<int> AccountIds { get; set; }

        public List<int> CategoryIds { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}

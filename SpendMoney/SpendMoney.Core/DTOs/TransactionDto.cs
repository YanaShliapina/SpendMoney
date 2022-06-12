using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.DTOs
{
    public class TransactionDto
    {
        public string TransactionId { get; set; }
        public string TransactionType { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}

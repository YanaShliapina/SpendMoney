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
        public string Description { get; set; }
        public int TransactionType { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int? TransferAccountId { get; set; }
        public int? UserDreamId { get; set; }
        public UserAccountDto UserAccount { get; set; }
        public CategoryDto Category { get; set; }
    }
}

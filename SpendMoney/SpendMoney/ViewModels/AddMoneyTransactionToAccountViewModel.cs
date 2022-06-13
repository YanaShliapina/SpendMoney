using SpendMoney.Core.DTOs;

namespace SpendMoney.ViewModels
{
    public class AddMoneyTransactionToAccountViewModel
    {
        public UserAccountDto Account { get; set; }
        public decimal AmountToAdd { get; set; }
        public string Description { get; set; }
    }
}

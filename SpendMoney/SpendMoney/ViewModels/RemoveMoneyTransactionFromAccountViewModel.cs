using SpendMoney.Core.DTOs;

namespace SpendMoney.ViewModels
{
    public class RemoveMoneyTransactionFromAccountViewModel
    {
        public UserAccountDto Account { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}

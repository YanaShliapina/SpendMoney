using SpendMoney.Core.DTOs;

namespace SpendMoney.ViewModels
{
    public class AddMoneyToCategoryViewModel
    {
        public int AccountId { get; set; }
        public CategoryDto Category { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public List<UserAccountDto> UserAccounts { get; set; }
    }
}

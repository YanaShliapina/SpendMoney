using SpendMoney.Core.DTOs;

namespace SpendMoney.ViewModels
{
    public class ChangeTransactionViewModel
    {
        public TransactionDto Transaction { get; set; }
        public List<UserAccountDto> UserAccounts { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }
}

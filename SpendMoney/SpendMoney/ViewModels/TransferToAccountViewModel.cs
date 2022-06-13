using SpendMoney.Core.DTOs;

namespace SpendMoney.ViewModels
{
    public class TransferToAccountViewModel
    {
        public UserAccountDto Account { get; set; }
        public List<UserAccountDto> UserAccounts { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int ToAccountId { get; set; }
    }
}

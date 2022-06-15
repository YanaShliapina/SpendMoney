using SpendMoney.Core.DTOs;

namespace SpendMoney.ViewModels
{
    public class TransactionHistoryViewModel
    {
        public List<UserAccountDto> UserAccounts { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public string AccountId { get; set; }
        public string CategoryId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<TransactionDto> Transactions { get; set; }

        public string CategoryListForChart { get; set; }
        public string CategoryValueListForChart { get; set; }
    }
}

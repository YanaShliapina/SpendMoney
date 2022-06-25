using SpendMoney.Core.DTOs;

namespace SpendMoney.ViewModels
{
    public class ChargeForDreamViewModel
    {
        public List<UserAccountDto> UserAccounts { get; set; }
        public int DreamId { get; set; }
        public string DreamName { get; set; }
        public int SelectedAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}

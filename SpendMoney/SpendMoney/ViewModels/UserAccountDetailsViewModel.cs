using SpendMoney.Core.DTOs;

namespace SpendMoney.ViewModels;

public class UserAccountDetailsViewModel
{
    public List<TransactionDto> Transactions { get; set; }
    public List<UserAccountDto> UserAccounts { get; set; }
    public List<CategoryDto> UserCategoryList { get; set; }
    public List<UserDreamDto> UserDreams { get; set; }
}
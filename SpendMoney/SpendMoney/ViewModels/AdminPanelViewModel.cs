using SpendMoney.Core.DTOs;

namespace SpendMoney.ViewModels
{
    public class AdminPanelViewModel
    {
        public List<CategoryDto> CategoryList { get; set; }
        public decimal SpendAmount { get; set; }
        public decimal AddAmount { get; set; }
    }
}

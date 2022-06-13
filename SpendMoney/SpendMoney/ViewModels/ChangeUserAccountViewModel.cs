using SpendMoney.Core.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SpendMoney.ViewModels
{
    public class ChangeUserAccountViewModel
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyShortName { get; set; }
        public string Color { get; set; }

        [Required]
        public int CurrencyId { get; set; }
        [Required]
        public int ImageId { get; set; }
        public List<ImageDto> Images { get; set; }
        public List<CurrencyDto> Currencies { get; set; }
    }
}

using SpendMoney.Core.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SpendMoney.ViewModels
{
    public class CreateUserAccountViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int CurrencyId { get; set; }
        [Required]
        public int ImageId { get; set; }
        public string Amount { get; set; }
        public string Color { get; set; }
        public List<ImageDto> Images { get; set; }
        public List<CurrencyDto> Currencies { get; set; }
    }
}

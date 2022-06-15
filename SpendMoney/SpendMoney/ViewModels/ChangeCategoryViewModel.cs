using SpendMoney.Core.DTOs;

namespace SpendMoney.ViewModels
{
    public class ChangeCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Image { get; set; }
        public decimal Amount { get; set; }

        public int ImageId { get; set; }
        public List<ImageDto> Images { get; set; }
    }
}

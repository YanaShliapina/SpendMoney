using SpendMoney.Core.DTOs;
using System.ComponentModel.DataAnnotations;

namespace SpendMoney.ViewModels;

public class CreateCategoryViewModel
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public List<ImageDto> Images { get; set; }
    public int SelectedImageId { get; set; }
}
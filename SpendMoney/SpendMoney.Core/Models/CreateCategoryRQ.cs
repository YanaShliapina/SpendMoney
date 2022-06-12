using System.ComponentModel.DataAnnotations;

namespace SpendMoney.Core.Models;

public class CreateCategoryRQ
{
    public String Name { get; set; }
    public String Description { get; set; }
    public string Color { get; set; }
    public int SelectedImageId { get; set; }
    public string UserId { get; set; }
}
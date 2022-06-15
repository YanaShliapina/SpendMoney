namespace SpendMoney.Core.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public string Image { get; set; }
    public decimal Amount { get; set; }
    public int ImageId { get; set; }
}
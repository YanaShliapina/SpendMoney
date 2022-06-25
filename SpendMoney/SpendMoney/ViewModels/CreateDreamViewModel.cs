namespace SpendMoney.ViewModels
{
    public class CreateDreamViewModel
    {
        public string Name { get; set; }
        public int SaveType { get; set; }
        public DateTime CurrentDate { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
    }
}

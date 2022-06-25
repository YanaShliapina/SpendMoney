namespace SpendMoney.ViewModels
{
    public class UserDreamViewModel
    {
        public string Name { get; set; }
        public int SaveType { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal Percentage { get; set; }
    }
}

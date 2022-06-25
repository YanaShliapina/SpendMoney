using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.DTOs
{
    public class UserDreamDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SaveType { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal EachPayAmount { get; set; }
        public decimal Percentage { get; set; }
    }
}

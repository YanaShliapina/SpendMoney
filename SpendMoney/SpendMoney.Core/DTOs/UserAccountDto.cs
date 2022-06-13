﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.DTOs
{
    public class UserAccountDto
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ImageId { get; set; }
        public string Image { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyShortName { get; set; }
        public string Color { get; set; }
    }
}
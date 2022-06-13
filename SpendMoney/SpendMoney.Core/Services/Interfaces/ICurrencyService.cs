using SpendMoney.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.Services.Interfaces
{
    public interface ICurrencyService
    {
        Task<List<CurrencyDto>> GetCurrencies();
    }
}

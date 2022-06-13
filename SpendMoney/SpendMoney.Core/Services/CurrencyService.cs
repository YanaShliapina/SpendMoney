using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Entities;
using SpendMoney.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CurrencyService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CurrencyDto>> GetCurrencies()
        {
            var foundCurrencies =  await _context.Currencies.ToListAsync();
            return _mapper.Map<List<CurrencyDto>>(foundCurrencies);
        }
    }
}

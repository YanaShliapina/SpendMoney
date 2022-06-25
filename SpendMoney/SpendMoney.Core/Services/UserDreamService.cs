using AutoMapper;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Entities;
using SpendMoney.Core.Models;
using SpendMoney.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.Services
{
    public class UserDreamService : IUserDreamService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserDreamService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateDream(CreateDreamRQ request)
        {
            var entity = _mapper.Map<UserDream>(request);

            var needAmount = request.TargetAmount - request.CurrentAmount;
            var dayCount = (request.EndDate.Date - DateTime.Now.Date).TotalDays;
            decimal amount = 0;

            if (entity.SaveType == 1)
            {
                var weekCount = Math.Round(((decimal)dayCount) / 7, 1);
                amount = Math.Round(needAmount / weekCount);
            }
            else if (entity.SaveType == 2)
            {
                var monthCount = Math.Round(((decimal)dayCount) / 30, 1);
                amount = Math.Round(needAmount / monthCount, 2);

            }
            else if (entity.SaveType == 3)
            {
                amount = Math.Round(needAmount / (decimal)dayCount, 2);
            }

            entity.EachPayAmount = amount;

            await _context.UserDreams.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserDreamDto>> GetDreamByUserId(string userId)
        {
            var dreams = _context.UserDreams.Where(x => x.UserId == userId).ToList();

            return _mapper.Map<List<UserDreamDto>>(dreams);
        }

        public async Task<UserDreamDto> GetDreamId(int id)
        {
            var dreams = _context.UserDreams.FirstOrDefault(x => x.Id == id);

            return _mapper.Map<UserDreamDto>(dreams);
        }
    }
}

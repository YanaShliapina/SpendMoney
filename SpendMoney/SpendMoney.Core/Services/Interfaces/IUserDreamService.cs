using SpendMoney.Core.DTOs;
using SpendMoney.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.Services.Interfaces
{
    public interface IUserDreamService
    {
        Task CreateDream(CreateDreamRQ request);
        Task<List<UserDreamDto>> GetDreamByUserId(string userId);
        Task<UserDreamDto> GetDreamId(int id);
    }
}

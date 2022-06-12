﻿using Microsoft.AspNetCore.Identity;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SpendMoney.Core.Services.Interfaces
{
    public interface IAccountServicecs
    {
        Task<ApplicationUser> Login(LoginDto loginRequest);
        Task<IdentityResult> Register(RegisterDto registerRequest);
        Task Logout();
        Task<List<string>> GetRoleListAsync(ApplicationUser user);
        Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal claimsPrincipal);
        Task<List<UserAccountDto>> GetAccountsByUserId(string userId);
        Task<List<TransactionDto>> GetTransactionList(TransactionFilter filter);
    }
}

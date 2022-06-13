using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Entities;
using SpendMoney.Core.Models;
using SpendMoney.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SpendMoney.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace SpendMoney.Core.Services
{
    public class AccountService : IAccountServicecs
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public AccountService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            IMapper mapper, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<string>> GetRoleListAsync(ApplicationUser user) =>
            (await _userManager.GetRolesAsync(user)).ToList();

        public async Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal claimsPrincipal) =>
            await _userManager.GetUserAsync(claimsPrincipal);

        public async Task<ApplicationUser> Login(LoginDto loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return user;
            }

            return null;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> Register(RegisterDto registerRequest)
        {
            var user = _mapper.Map<ApplicationUser>(registerRequest);
            user.ProfileImage = "/default_profile_image.png";
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            var registeredUser = await _userManager.FindByEmailAsync(registerRequest.Email);
            await _userManager.AddToRoleAsync(registeredUser, AuthorizationConstants.Roles.CLIENT);
            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<List<UserAccountDto>> GetAccountsByUserId(string userId)
        {
            var foundUserAccounts = await _context.UserMoneyAccounts
                    .Include(x => x.TransactionAccounts)
                    .Include(x => x.Account)
                    .ThenInclude(x => x.Image)
                    .Include(x => x.Account)
                    .ThenInclude(x => x.Currency)
                    .Where(x => x.UserId == userId).ToListAsync();

            return _mapper.Map<List<UserAccountDto>>(foundUserAccounts);
        }

        public async Task<List<TransactionDto>> GetTransactionList(TransactionFilter filter)
        {
            var foundTrans = await _context.Transactions
                .Include(x => x.Category)
                .Include(x => x.Account)
                .ThenInclude(x => x.Account)
                .Include(x => x.TypeNavigation)
                .Where(x => x.UserId == filter.UserId).ToListAsync();

            return _mapper.Map<List<TransactionDto>>(foundTrans);
        }

        public async Task<UserAccountDto> CreateUserAccount(CreateUserAccountRQ request)
        {
            var moneyAccount = _mapper.Map<MoneyAccount>(request);

            moneyAccount.Image = await _context.Images.FirstOrDefaultAsync(x => x.Id == request.ImageId);
            moneyAccount.Currency = await _context.Currencies.FirstOrDefaultAsync(x => x.Id == request.CurrencyId);

            await _context.MoneyAccounts.AddAsync(moneyAccount);
            await _context.SaveChangesAsync();

            var userMoneyAccount = _mapper.Map<UserMoneyAccount>(request);
            userMoneyAccount.Account = moneyAccount;
            userMoneyAccount.User = await _context.AspNetUsers.FirstOrDefaultAsync(x => x.Id == request.UserId);

            await _context.UserMoneyAccounts.AddAsync(userMoneyAccount);
            await _context.SaveChangesAsync();

            return new UserAccountDto();
        }

        public async Task<UserAccountDto> GetAccountById(int accountId)
        {
            var foundAccount = await _context.UserMoneyAccounts
                .Include(x => x.TransactionAccounts)
                .Include(x => x.Account)
                .ThenInclude(x => x.Image)
                .Include(x => x.Account)
                .ThenInclude(x => x.Currency)
                .FirstOrDefaultAsync(x => x.AccountId == accountId);

            return _mapper.Map<UserAccountDto>(foundAccount);
        }

        public async Task AddAmount(int accountId, decimal amountToAdd)
        {
            var foundAccount = await _context.UserMoneyAccounts
                .FirstOrDefaultAsync(x => x.AccountId == accountId);

            foundAccount.Amount += amountToAdd;

            await _context.SaveChangesAsync();
        }

        public async Task RemoveAmount(int accountId, decimal amountToRemove)
        {
            var foundAccount = await _context.UserMoneyAccounts
                .FirstOrDefaultAsync(x => x.AccountId == accountId);

            foundAccount.Amount -= amountToRemove;

            await _context.SaveChangesAsync();
        }

        public async Task<UserAccountDto> UpdateUserAccount(UpdateUserAccountRQ request)
        {
            UserMoneyAccount currentUserAccount = await _context.UserMoneyAccounts
                 .Include(x => x.TransactionAccounts)
                 .Include(x => x.Account)
                 .ThenInclude(x => x.Image)
                 .Include(x => x.Account)
                 .ThenInclude(x => x.Currency)
                 .FirstAsync(x => x.AccountId == request.AccountId);

            if (currentUserAccount.Account.CurrencyId != request.CurrencyId && (currentUserAccount?.Amount ?? 0) > 0)
            {
                var convertRate = await _context.CurrencyExchanges
                                    .Include(x => x.FromNavigation)
                                    .Include(x => x.ToNavigation)
                                    .FirstOrDefaultAsync(x => x.FromNavigation.Id == currentUserAccount.Account.CurrencyId &&
                                        x.ToNavigation.Id == request.CurrencyId);

                if(convertRate == null)
                {
                    return null;
                }

                currentUserAccount.Amount = Math.Round((decimal)currentUserAccount.Amount * convertRate.Rate, 2);
            }

            currentUserAccount.Account.Color = request.Color;
            currentUserAccount.Account.Description = request.Description;
            currentUserAccount.Account.CurrencyId = request.CurrencyId;
            currentUserAccount.Account.ImageId = request.ImageId;
            currentUserAccount.Account.Name = request.Name;

            await _context.SaveChangesAsync();

            return _mapper.Map<UserAccountDto>(currentUserAccount);
        }
    }
}

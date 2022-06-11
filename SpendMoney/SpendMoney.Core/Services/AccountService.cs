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
            user.FirstName = "";
            user.LastName = "";
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            var registeredUser = await _userManager.FindByEmailAsync(registerRequest.Email);
            await _userManager.AddToRoleAsync(registeredUser, AuthorizationConstants.Roles.CLIENT);
            await _context.SaveChangesAsync();

            return result;
        }
    }
}

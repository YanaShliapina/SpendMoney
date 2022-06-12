using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SpendMoney.Core.Constants;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Entities;
using SpendMoney.Core.Models;
using SpendMoney.Core.Services.Interfaces;
using SpendMoney.ViewModels;

namespace SpendMoney.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountServicecs _accountService;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(IAccountServicecs accountService, IMapper mapper, 
            ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _accountService = accountService;
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
        }


        [Route("Index")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginRequest)
        {
            var request = _mapper.Map<LoginDto>(loginRequest);
            var user = await _accountService.Login(request);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                return View(loginRequest);
            }

            var roleList = await _accountService.GetRoleListAsync(user);

            if (roleList.Contains(AuthorizationConstants.Roles.ADMIN))
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private async Task<IdentityResult> AssignRoles(string email, string role)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            var result = await _userManager.AddToRoleAsync(user, role);

            return result;
        }

        [Route("Logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout();
            return RedirectToAction("Login", "Account");
        }

        [Route("Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerRequest)
        {
            if (registerRequest.Password != registerRequest.SecondPassword)
            {
                ModelState.AddModelError(nameof(RegisterViewModel.SecondPassword), "Mismatch");
                return View(registerRequest);
            }
            
            var request = _mapper.Map<RegisterDto>(registerRequest);
            var result = await _accountService.Register(request);

            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            return View();
        }
    }
}

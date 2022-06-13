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
        private readonly ICurrencyService _currencyService;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(IAccountServicecs accountService, IMapper mapper,
            ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            ICurrencyService currencyService, IImageService imageService)
        {
            _accountService = accountService;
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _currencyService = currencyService;
            _imageService = imageService;
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

        [HttpGet]
        [Route("CreateUserAccount")]
        public async Task<IActionResult> CreateUserAccount()
        {
            var foundCurrencies = await _currencyService.GetCurrencies();
            var images = await _imageService.GetUserAccountImageList();

            var viewModel = new CreateUserAccountViewModel
            {
                Currencies = new List<CurrencyDto>(foundCurrencies),
                Images = new List<ImageDto>(images)
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("CreateUserAccount")]
        public async Task<IActionResult> CreateUserAccount(CreateUserAccountViewModel createUserAccountViewModel)
        {
            ModelState.Remove("Images");
            ModelState.Remove("Currencies");

            if (ModelState.IsValid == false)
            {
                var foundCurrencies = await _currencyService.GetCurrencies();
                var images = await _imageService.GetUserAccountImageList();

                createUserAccountViewModel.Currencies = new List<CurrencyDto>(foundCurrencies);
                createUserAccountViewModel.Images = new List<ImageDto>(images);

                return View(createUserAccountViewModel);
            }

            var request = _mapper.Map<CreateUserAccountRQ>(createUserAccountViewModel);
            var user = await _accountService.GetCurrentUser(User);
            request.UserId = user.Id;

            await _accountService.CreateUserAccount(request);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("ChangeUserAccount")]
        public async Task<IActionResult> ChangeUserAccount(int id)
        {
            var account = await _accountService.GetAccountById(id);
            var foundCurrencies = await _currencyService.GetCurrencies();
            var images = await _imageService.GetUserAccountImageList();

            var viewModel = _mapper.Map<ChangeUserAccountViewModel>(account);
            viewModel.CurrencyId = foundCurrencies.First(x => x.ShortName == account.CurrencyShortName).Id;

            viewModel.Images = new List<ImageDto>(images);
            viewModel.Currencies = new List<CurrencyDto>(foundCurrencies);

            return View(viewModel);
        }

        [HttpPost]
        [Route("ChangeUserAccount")]
        public async Task<IActionResult> ChangeUserAccount(ChangeUserAccountViewModel viewModel)
        {
            ModelState.Remove("Images");
            ModelState.Remove("Currencies");
            ModelState.Remove("Image");
            ModelState.Remove("CurrencyShortName");

            if (ModelState.IsValid == false)
            {
                var foundCurrencies = await _currencyService.GetCurrencies();
                var images = await _imageService.GetUserAccountImageList();

                viewModel.Currencies = new List<CurrencyDto>(foundCurrencies);
                viewModel.Images = new List<ImageDto>(images);

                return View(viewModel);
            }

            //update account details
            var request = _mapper.Map<UpdateUserAccountRQ>(viewModel);
            await _accountService.UpdateUserAccount(request);

            return RedirectToAction("Index", "Home");
        }
    }
}

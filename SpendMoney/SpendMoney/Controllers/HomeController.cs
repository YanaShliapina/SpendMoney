﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Packaging;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Models;
using SpendMoney.Core.Services.Interfaces;
using SpendMoney.ViewModels;

namespace SpendMoney.Controllers
{
    [Authorize]
    [Route("Home")]
    public class HomeController : Controller
    {
        private readonly IAccountServicecs _accountService;
        private readonly ICategoryService _categoryService;
        private readonly IImageService _imageService;
        private readonly IUserDreamService _userDreamService;

        public HomeController(IAccountServicecs accountService, ICategoryService categoryService, IImageService imageService, IUserDreamService userDreamService)
        {
            _accountService = accountService;
            _categoryService = categoryService;
            _imageService = imageService;
            _userDreamService = userDreamService;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var user = await _accountService.GetCurrentUser(User);
            var userAccounts = await _accountService.GetAccountsByUserId(user.Id);
            var foundCats = await _categoryService.GetCategoryListByUserId(user.Id);
            var userTrans = await _accountService.GetTransactionList(new TransactionFilter
            {
                UserId = user.Id
            });
            var userDreams = await _userDreamService.GetDreamByUserId(user.Id);

            var viewModel = new UserAccountDetailsViewModel()
            {
                UserAccounts = new List<UserAccountDto>(userAccounts),
                UserCategoryList = new List<CategoryDto>(foundCats),
                Transactions = new List<TransactionDto>(userTrans),
                UserDreams = userDreams
            };

            return View(viewModel);
        }
    }
}

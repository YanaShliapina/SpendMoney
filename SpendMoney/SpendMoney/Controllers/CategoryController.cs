using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Models;
using SpendMoney.Core.Services.Interfaces;
using SpendMoney.ViewModels;

namespace SpendMoney.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IMapper _mapper;
        private ICategoryService _categoryService;
        private IAccountServicecs _accountService;
        private IImageService _imageService;

        public CategoryController(IMapper mapper, ICategoryService categoryService, IAccountServicecs accountService, IImageService imageService)
        {
            _mapper = mapper;
            _categoryService = categoryService;
            _accountService = accountService;
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("CreateCategory")]
        [HttpGet]
        public async Task<IActionResult> CreateCategory()
        {
            var foundImages = await _imageService.GetCategoryImageList();

            var viewModel = new CreateCategoryViewModel()
            {
                Images = new List<ImageDto>(foundImages)
            };
            
            return View(viewModel);
        }
        
        [Route("CreateCategory")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel createCategoryViewModel)
        {
            var request = _mapper.Map<CreateCategoryRQ>(createCategoryViewModel);
            request.UserId = (await _accountService.GetCurrentUser(User)).Id;
            
            try
            {
                await _categoryService.CreateCategory(request);
            }
            catch (Exception ex)
            {
                //log
            }
            
            return RedirectToAction("Index", "Home");
        }
    }
}

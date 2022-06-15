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

        [Route("ChangeCategory")]
        [HttpGet]
        public async Task<IActionResult> ChangeCategory(int categoryId)
        {
            var user = await _accountService.GetCurrentUser(User);
            var cat = await _categoryService.GetCategoryById(categoryId, user.Id);
            var images = await _imageService.GetCategoryImageList();

            var viewModel = _mapper.Map<ChangeCategoryViewModel>(cat);
            viewModel.Images = images;

            return View(viewModel);
        }

        [Route("ChangeCategory")]
        [HttpPost]
        public async Task<IActionResult> ChangeCategory(ChangeCategoryViewModel viewModel)
        {
            ModelState.Remove("Images");
            ModelState.Remove("Image");

            var user = await _accountService.GetCurrentUser(User);

            if (ModelState.IsValid == false)
            {
                var cat = await _categoryService.GetCategoryById(viewModel.Id, user.Id);
                var images = await _imageService.GetCategoryImageList();

                viewModel.Images = new List<ImageDto>(images);

                return View(viewModel);
            }

            var request = _mapper.Map<UpdateCategoryRQ>(viewModel);
            request.UserId = user.Id;
            await _categoryService.UpdateCategory(request);

            return RedirectToAction("Index", "Home");
        }
    }
}

using SpendMoney.Core.DTOs;
using SpendMoney.Core.Entities;
using SpendMoney.Core.Models;

namespace SpendMoney.Core.Services.Interfaces;

public interface ICategoryService
{
    Task<CategoryDto> CreateCategory(CreateCategoryRQ request);
    Task<List<CategoryDto>> GetCategoryListByUserId(string userId);
    Task<CategoryDto> GetCategoryById(int categoryId, string userId = "");
    Task<CategoryDto> UpdateCategory(UpdateCategoryRQ request);
    Task<Category> GetCategoryEntityById(int categoryId, string userId = "");
    Task<List<CategoryDto>> GetAllCategoryList();
}
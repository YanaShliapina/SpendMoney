using SpendMoney.Core.DTOs;

namespace SpendMoney.Core.Services.Interfaces;

public interface IImageService
{
    Task<List<ImageDto>> GetCategoryImageList();
    Task<List<ImageDto>> GetUserAccountImageList();
}
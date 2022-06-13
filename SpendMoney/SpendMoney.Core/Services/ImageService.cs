using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpendMoney.Core.Constants;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Entities;

namespace SpendMoney.Core.Services.Interfaces;

public class ImageService : IImageService
{
    private ApplicationDbContext _context;
    private IMapper _mapper;
    private enum ImageType
    {
        Category,
        UserAccount
    }
    
    public ImageService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ImageDto>> GetCategoryImageList() =>
        await GetImagesByType(ImageType.Category);

    public async Task<List<ImageDto>> GetUserAccountImageList() =>
        await GetImagesByType(ImageType.UserAccount);

    private async Task<List<ImageDto>> GetImagesByType(ImageType type)
    {
        var foundImages = await _context.Images.Where(x => x.Type == (int)type).ToListAsync();
        var preparedImages = _mapper.Map<List<ImageDto>>(foundImages);

        foreach (var preparedImage in preparedImages)
        {
            preparedImage.Path = string.Concat(ImageConstants.ROOT_PATH, preparedImage.Path);
        }

        return preparedImages;
    }
}
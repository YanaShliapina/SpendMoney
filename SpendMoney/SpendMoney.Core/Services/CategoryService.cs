﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpendMoney.Core.Constants;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Entities;
using SpendMoney.Core.Models;
using SpendMoney.Core.Services.Interfaces;

namespace SpendMoney.Core.Services;

public class CategoryService : ICategoryService
{
    private ApplicationDbContext _context;
    private readonly IMapper _mapper;
    
    public CategoryService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<CategoryDto> CreateCategory(CreateCategoryRQ request)
    {
        var newCategory = _mapper.Map<Category>(request);
        //image
        var assignedImage = await _context.Images
            .FirstOrDefaultAsync(x => x.Id == request.SelectedImageId);

        if (assignedImage == null)
        {
            assignedImage = await _context.Images
                .FirstOrDefaultAsync(x => x.Name == ImageConstants.DEFAULT_CATEGORY_IMAGE_NAME);
        }

        newCategory.Image = assignedImage;
        
        await _context.Categories.AddAsync(newCategory);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<CategoryDto>(newCategory);
    }

    public async Task<List<CategoryDto>> GetCategoryListByUserId(string userId)
    {
        var foundCats = await _context.Categories
            .Include(x => x.Image)
            .Include(x => x.Transactions.Where(t => t.UserId == userId))
            .Where(x => x.UserId == userId || string.IsNullOrEmpty(x.UserId))
            .ToListAsync();
        return _mapper.Map<List<CategoryDto>>(foundCats);
    }
}
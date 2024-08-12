using AutoMapper;
using GameService.Base;
using GameService.Data;
using GameService.DTOs;
using GameService.Entities;
using  GameService.Repositories.ForCategory;
using Microsoft.EntityFrameworkCore;

namespace GameService.Repositories.ForCategory;


public class CategoryRepository : ICategoryRepository
{
    private readonly GameDbContext _context;
    private IMapper _mapper;

    private BaseResponseModel _responseModel;

    public CategoryRepository(IMapper mapper, GameDbContext context, BaseResponseModel responseModel)
    {
        _mapper = mapper;
        _context = context;
        _responseModel = responseModel;
    }

    public async Task<BaseResponseModel> CreateCategory(CategoryDTO category)
    {
       var objDTO = _mapper.Map<Category>(category);
        await _context.Categories.AddAsync(objDTO);

        if(await _context.SaveChangesAsync() > 0){
         _responseModel.IsSuccess = true;
         _responseModel.Message = "SuccessFull";
         _responseModel.Data = objDTO;
         return _responseModel;
        }

        _responseModel.IsSuccess = false;
        return _responseModel;
    }

    public async Task<BaseResponseModel> GetAllCategories()
    {
       List<Category> categories = await _context.Categories.ToListAsync();

       if(categories is not null){
        _responseModel.Data = categories;
        _responseModel.IsSuccess = true;
        return _responseModel;
       }

       _responseModel.IsSuccess = false;
       return _responseModel;
    }

    public async Task<bool> RemoveCategory(Guid categoryId)
    {
        Category category = await _context.Categories.FindAsync(categoryId);
        if (category is not null)
        {
            _context.Categories.Remove(category);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
        }
        return false;
    }

    public async Task<BaseResponseModel> UpdateCategory(CategoryDTO category, Guid categoryId)
    {
        var obj = await _context.Categories.FindAsync(categoryId);
       
       if(obj is not null){
          obj.CategoryDescription = category.CategoryDescription;
          obj.CategoryName = category.CategoryName;
       }

       if (await _context.SaveChangesAsync() > 0)
       {
        _responseModel.Data = category;
        _responseModel.IsSuccess = true;
        _responseModel.Message = "Success";
        return _responseModel;
       }

        _responseModel.IsSuccess = false;
        return _responseModel;

    }
}
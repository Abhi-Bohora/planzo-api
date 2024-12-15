using AutoMapper;
using Planzo.Data.Dtos;
using Planzo.Data.Dtos.Category;
using Planzo.Data.Models;
using Planzo.Data.UnitOfWork;
using Planzo.Service.Interfaces;

namespace Planzo.Service;

public class CategoryService:ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;   
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseDto<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto, string userId)
    {
        try
        {
            var category = _mapper.Map<Category>(createCategoryDto);
            category.UserId = userId;
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;
            
            var categoryRepo = _unitOfWork.Repository<Category>();
            await categoryRepo.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            
            var createdCategoryDto = _mapper.Map<CategoryDto>(category);
            return new ResponseDto<CategoryDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Category Created Successfully!",
                Data = createdCategoryDto
            };
        }
        catch (Exception e)
        {
            return new ResponseDto<CategoryDto>
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = $"An error occurred while creating the category: {e.Message}",
                Data = null
            };
        }
    }

    public async Task<ResponseDto<CategoryDto>> UpdateCategoryAsync(int categoryId, CreateCategoryDto createCategoryDto, string userId)
    {
        try
        {
            var categoryRepo = _unitOfWork.Repository<Category>();
            var existingCategory = await categoryRepo.GetAsync(c => c.Id == categoryId && c.UserId == userId);

            if (existingCategory == null)
            {
                return new ResponseDto<CategoryDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Category not found.",
                    Data = null
                };
            }
            _mapper.Map(createCategoryDto, existingCategory);
            existingCategory.UpdatedAt = DateTime.UtcNow;

            categoryRepo.Update(existingCategory);
            await _unitOfWork.SaveChangesAsync();

            var updatedCategoryDto = _mapper.Map<CategoryDto>(existingCategory);
            return new ResponseDto<CategoryDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Category Updated Successfully!",
                Data = updatedCategoryDto
            };
        }
        catch (Exception e)
        {
            return new ResponseDto<CategoryDto>
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = $"An error occurred while updating the category: {e.Message}",
                Data = null
            };
        }
    }

    public async Task<ResponseDto<CategoryDto>> DeleteCategoryAsync(int categoryId, string userId)
    {
        try
        {
            var categoryRepo = _unitOfWork.Repository<Category>();
            var existingCategory = await categoryRepo.GetAsync(c => c.Id == categoryId && c.UserId == userId);

            if (existingCategory == null)
            {
                return new ResponseDto<CategoryDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Category not found.",
                    Data = null
                };
            }
            
            var projectRepo = _unitOfWork.Repository<Project>();
            var projectsUsingCategory = await projectRepo.GetAllAsync(p => p.CategoryId == categoryId);

            if (projectsUsingCategory.Any())
            {
                return new ResponseDto<CategoryDto>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Cannot delete category. It is currently used in one or more projects.",
                    Data = null
                };
            }

            await categoryRepo.DeleteAsync(existingCategory);
            await _unitOfWork.SaveChangesAsync();

            return new ResponseDto<CategoryDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Category Deleted Successfully!",
                Data = null
            };
        }
        catch (Exception e)
        {
            return new ResponseDto<CategoryDto>
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = $"An error occurred while deleting the category: {e.Message}",
                Data = null
            };
        }
    }

    public async Task<ResponseDto<CategoryDto>> GetCategoryByIdAsync(int categoryId, string userId)
    {
        try
        {
            var categoryRepo = _unitOfWork.Repository<Category>();
            var category = await categoryRepo.GetAsync(c => c.Id == categoryId && c.UserId == userId);

            if (category == null)
            {
                return new ResponseDto<CategoryDto>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Category not found.",
                    Data = null
                };
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return new ResponseDto<CategoryDto>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Category Retrieved Successfully!",
                Data = categoryDto
            };
        }
        catch (Exception e)
        {
            return new ResponseDto<CategoryDto>
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving the category: {e.Message}",
                Data = null
            };
        }
    }

    public async Task<ResponseDto<List<CategoryDto>>> GetAllCategoriesAsync(string userId)
    {
        try
        {
            var categoryRepo = _unitOfWork.Repository<Category>();
            var categories = await categoryRepo.GetAllAsync(c => c.UserId == userId);

            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return new ResponseDto<List<CategoryDto>>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Categories Retrieved Successfully!",
                Data = categoryDtos
            };
        }
        catch (Exception e)
        {
            return new ResponseDto<List<CategoryDto>>
            {
                IsSuccess = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving categories: {e.Message}",
                Data = null
            };
        }
    }
}
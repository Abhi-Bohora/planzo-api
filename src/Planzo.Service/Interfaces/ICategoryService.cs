using Planzo.Data.Dtos;
using Planzo.Data.Dtos.Category;

namespace Planzo.Service.Interfaces;

public interface ICategoryService
{
    Task<ResponseDto<CategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto, string userId);
    Task<ResponseDto<CategoryDto>> UpdateCategoryAsync(int categoryId, CreateCategoryDto createCategoryDto, string userId);
    Task<ResponseDto<CategoryDto>> DeleteCategoryAsync(int categoryId, string userId);
    Task<ResponseDto<CategoryDto>> GetCategoryByIdAsync(int categoryId, string userId);
    Task<ResponseDto<List<CategoryDto>>> GetAllCategoriesAsync(string userId);
}
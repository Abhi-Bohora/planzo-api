using System.ComponentModel.DataAnnotations;

namespace Planzo.Data.Dtos.Category;

public class CreateCategoryDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }    
    
    [StringLength(100)]
    public string? Description { get; set; }
}
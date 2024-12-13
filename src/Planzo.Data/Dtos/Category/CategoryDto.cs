using System.ComponentModel.DataAnnotations;

namespace Planzo.Data.Dtos.Category;

public class CategoryDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; } 
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } 
}
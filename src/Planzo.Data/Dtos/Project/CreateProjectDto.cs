using System.ComponentModel.DataAnnotations;
using Planzo.Data.Models;

namespace Planzo.Data.Dtos.Project;

public class CreateProjectDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public int? CategoryId { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
    
    public DateTime StartDate { get; set; } 
    public DateTime EstimateEndDate {get; set;} 
}
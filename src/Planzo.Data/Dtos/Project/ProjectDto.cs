using Planzo.Data.Models;

namespace Planzo.Data.Dtos.Project;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EstimateEndDate { get; set; }
    public string UserId { get; set; }
    public int CategoryId { get; set; }   
    public ProjectStatus Status { get; set; }   
    public DateTime CreatedAt { get; set; } 
    public DateTime UpdatedAt { get; set; }
}
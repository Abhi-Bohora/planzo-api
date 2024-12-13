using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Planzo.Data.Models;

public class Project
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }    
    
    [Required]
    [StringLength(500)]
    public string Description { get; set; } 
    
    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
    
    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
    public DateTime StartDate { get; set; } = DateTime.Now; 
    public DateTime DueDate { get; set; } 
}

public enum ProjectStatus
{
    NotStarted,
    InProgress,
    Completed,
    Cancelled
}
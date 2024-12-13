using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Planzo.Data.Models;

public class Category
{
    [Key]
    public int Id { get; set; } 
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Description { get; set; } 
    
    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; }
    public IdentityUser User { get; set; }  
    public ICollection<Project> Projects { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; } 
}
using System.ComponentModel.DataAnnotations;

namespace Shop.Contract.Requests;

public class CreateCategoryDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [StringLength(200)]
    public string? Description { get; set; }
}

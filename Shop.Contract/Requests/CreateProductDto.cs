using System.ComponentModel.DataAnnotations;

namespace Shop.Contract.Requests;

public class CreateProductDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be positive")]
    public int CategoryId { get; set; }
}

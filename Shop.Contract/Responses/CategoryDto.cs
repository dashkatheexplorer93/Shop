using System.ComponentModel.DataAnnotations;

namespace Shop.Contract.Responses;

public class CategoryDto
{
    public int CategoryId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    public IEnumerable<ProductDto> Products { get; set; }
}

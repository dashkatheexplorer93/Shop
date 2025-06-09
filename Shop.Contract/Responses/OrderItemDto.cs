using System.ComponentModel.DataAnnotations;

namespace Shop.Contract.Responses;

public class OrderItemDto
{
    public int OrderItemId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ProductId must be positive")]
    public int ProductId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string ProductName { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be positive")]
    public int Quantity { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
}

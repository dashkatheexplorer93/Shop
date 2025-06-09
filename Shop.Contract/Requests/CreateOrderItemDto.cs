using System.ComponentModel.DataAnnotations;

namespace Shop.Contract.Requests;

public class CreateOrderItemDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ProductId must be positive")]
    public int ProductId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be positive")]
    public int Quantity { get; set; }
}

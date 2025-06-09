using System.ComponentModel.DataAnnotations;

namespace Shop.Contract.Requests;

public class CreateOrderDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be positive")]
    public int CustomerId { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "Order must contain at least one item")]
    public ICollection<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
}


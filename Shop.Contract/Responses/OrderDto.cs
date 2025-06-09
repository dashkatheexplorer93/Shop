using System.ComponentModel.DataAnnotations;

namespace Shop.Contract.Responses;

public class OrderDto
{
    public int OrderId { get; set; }
    
    [Required]
    public DateTime OrderDate { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "TotalPrice must be greater than 0")]
    public decimal TotalPrice { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be positive")]
    public int CustomerId { get; set; }

    public ICollection<OrderItemDto> OrderItems { get; set; }
}

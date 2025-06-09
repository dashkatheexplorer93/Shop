using System.ComponentModel.DataAnnotations;

namespace Shop.Contract.Responses;

public class CustomerDto
{
    public int CustomerId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FullName { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }
    
    [Phone]
    [StringLength(50)]
    public string? PhoneNumber { get; set; }
    
    [StringLength(200)]
    public string? Address { get; set; }
}

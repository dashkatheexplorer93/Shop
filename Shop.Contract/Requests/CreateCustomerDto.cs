using System.ComponentModel.DataAnnotations;

namespace Shop.Contract.Requests;

public class CreateCustomerDto
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    [StringLength(200)]
    public string? Address { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Customer;

public abstract class CreateOrUpdateCustomerDto
{
    [Required]
    public string? Username { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Maximum length for Customer first name is 100 characters.")]
    public string? FirstName { get; set; }

    [Required]
    [MaxLength(150, ErrorMessage = "Maximum length for Customer last name is 150 characters.")]
    public string? LastName { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Must be a valid email format.")]
    public string? EmailAddress { get; set; }
}
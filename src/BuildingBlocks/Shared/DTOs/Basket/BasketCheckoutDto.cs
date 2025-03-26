using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Basket;

public class BasketCheckoutDto
{
    [Required]
    public string? Username { get; set; }
    public decimal TotalPrice { get; set; }
        
    [Required]
    public string? FirstName { get; set; }
        
    [Required]
    public string? LastName { get; set; }
        
    [EmailAddress]
    public string? EmailAddress { get; set; }
        
    [Required]
    public string? ShippingAddress { get; set; }

    private string? _invoiceAddress;

    public string? InvoiceAddress
    {
        get => _invoiceAddress;
        set => _invoiceAddress = value ?? ShippingAddress;
    }
}
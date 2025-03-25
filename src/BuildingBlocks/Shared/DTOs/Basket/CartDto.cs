namespace Shared.DTOs.Basket;

public class CartDto
{
    public string? Username { get; set; }

    public string? EmailAddress { get; set; } // temporarily put it here

    public List<CartItemDto> Items { get; set; } = [];

    public CartDto()
    {
    }

    public CartDto(string? username)
    {
        Username = username;
    }

    public decimal TotalPrice => Items.Sum(item => item.ItemPrice * item.Quantity);
}
using EventBus.Messages.IntegrationEvents.Interfaces;

namespace EventBus.Messages.IntegrationEvents.Events;

// Only place here all Events relevant to Mass Transit(communicating between microservices)
// => we isolated them with Contracts
public record BasketCheckoutEvent : IntegrationBaseEvent, IBasketCheckoutEvent
{
    public string? Username { get; set; }
    public decimal TotalPrice { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public string? ShippingAddress { get; set; }
    public string? InvoiceAddress { get; set; }
}
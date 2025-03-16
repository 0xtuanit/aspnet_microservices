using Contracts.Common.Events;

namespace Ordering.Domain.OrderAggregate.Events;

public class OrderCreatedEvent : BaseEvent
{
    public long Id { get; private set; }
    public string? Username { get; private set; }
    public string? DocumentNo { get; private set; }
    public string? EmailAddress { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string? ShippingAddress { get; private set; }
    public string? InvoiceAddress { get; private set; }

    public OrderCreatedEvent(long id, string? username, string? documentNo, string? emailAddress, decimal totalPrice,
        string? shippingAddress, string? invoiceAddress)
    {
        Id = id;
        Username = username;
        DocumentNo = documentNo;
        EmailAddress = emailAddress;
        TotalPrice = totalPrice;
        ShippingAddress = shippingAddress;
        InvoiceAddress = invoiceAddress;
    }
}
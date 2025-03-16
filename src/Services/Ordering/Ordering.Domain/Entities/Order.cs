using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Common.Events;
using Ordering.Domain.Enums;
using Ordering.Domain.OrderAggregate.Events;

namespace Ordering.Domain.Entities;

// Inherited from AuditableEventEntity to apply DDD to this Order service
public class Order : AuditableEventEntity<long>
{
    [Required]
    [Column(TypeName = "nvarchar(150)")]
    public string? Username { get; set; }
    
    // Temporary solution for automatically inserting Document number
    public Guid DocumentNo { get; set; } = Guid.NewGuid();

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPrice { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string? FirstName { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(250)")]
    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    [Column(TypeName = "varchar(250)")]
    public string? EmailAddress { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? ShippingAddress { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? InvoiceAddress { get; set; }

    public EOrderStatus? Status { get; set; }

    public Order AddedOrder()
    {
        AddDomainEvent(new OrderCreatedEvent(Id, Username, DocumentNo.ToString(), EmailAddress, TotalPrice,
            ShippingAddress, InvoiceAddress));
        return this;
    }

    public Order DeletedOrder()
    {
        AddDomainEvent(new OrderDeletedEvent(Id));
        return this;
    }
}
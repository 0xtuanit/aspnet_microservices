using AutoMapper;
using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Application.Common.Models;
using Ordering.Domain.Entities;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class UpdateOrderCommand : IRequest<ApiResult<OrderDto>>, IMapFrom<Order>
{
    public long Id { get; private set; }

    public void SetId(long id)
    {
        Id = id;
    }

    public string? Username { get; set; }
    public decimal TotalPrice { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? EmailAddress { get; set; }

    public string? ShippingAddress { get; set; }

    public string? InvoiceAddress { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<UpdateOrderCommand, Order>();
    }
}
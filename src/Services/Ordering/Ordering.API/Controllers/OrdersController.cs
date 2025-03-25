using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.V1.Orders;
using Shared.DTOs.Order;
using Shared.SeedWork;
using OrderDto = Ordering.Application.Common.Models.OrderDto;

namespace Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    // private readonly ISmtpEmailService _emailService;

    public OrdersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        // _emailService = emailService;
    }

    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
        public const string GetOrder = nameof(GetOrder); // Get by Id
        public const string CreateOrder = nameof(CreateOrder);
        public const string UpdateOrder = nameof(UpdateOrder);
        public const string DeleteOrder = nameof(DeleteOrder);
        public const string DeleteOrderByDocumentNo = nameof(DeleteOrderByDocumentNo);
    }

    #region CRUD

    [HttpGet("{username}", Name = RouteNames.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUsername([Required] string username)
    {
        var query = new GetOrdersQuery(username);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:long}", Name = RouteNames.GetOrder)]
    [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<OrderDto>> GetOrder([Required] long id)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost(Name = RouteNames.CreateOrder)]
    [ProducesResponseType(typeof(ApiResult<long>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ApiResult<long>>> CreateOrder([FromBody] CreateOrderDto model)
    {
        var command = _mapper.Map<CreateOrderCommand>(model);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:long}", Name = RouteNames.UpdateOrder)]
    [ProducesResponseType(typeof(ApiResult<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ApiResult<OrderDto>>> UpdateOrder([Required] long id,
        [FromBody] UpdateOrderCommand command)
    {
        command.SetId(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:long}", Name = RouteNames.DeleteOrder)]
    [ProducesResponseType(typeof(NoContentResult), (int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeleteOrder([Required] long id)
    {
        var command = new DeleteOrderCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("document-no/{documentNo}", Name = RouteNames.DeleteOrderByDocumentNo)]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.NoContent)]
    public async Task<ApiResult<bool>> DeleteOrderByDocumentNo([Required] string documentNo)
    {
        var command = new DeleteOrderByDocumentNoCommand(documentNo);
        var result = await _mediator.Send(command);
        return result;
    }

    // [HttpGet("test-email")]
    // public async Task<IActionResult> TestEmail()
    // {
    //     var message = new MailRequest
    //     {
    //         Body = "<h1>Hello</h1>",
    //         Subject = "Testing",
    //         ToAddress = "tuanit168@gmail.com"
    //     };
    //     await _emailService.SendEmailAsync(message);
    //
    //     return Ok();
    // }

    #endregion
}
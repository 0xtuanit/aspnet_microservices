using System.ComponentModel.DataAnnotations;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;

namespace Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    // private readonly ISmtpEmailService _emailService;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        // _emailService = emailService;
    }

    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
    }

    [HttpGet("{username}", Name = RouteNames.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUsername([Required] string username)
    {
        var query = new GetOrdersQuery(username);
        var result = await _mediator.Send(query);
        return Ok(result);
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
}
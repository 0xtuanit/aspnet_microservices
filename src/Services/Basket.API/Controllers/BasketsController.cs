using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using Basket.API.GrpcServices;
using EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Shared.DTOs.Basket;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketsController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;
    private readonly StockItemGrpcService _stockItemGrpcService;

    public BasketsController(IBasketRepository basketRepository, IPublishEndpoint publishEndpoint, IMapper mapper,
        StockItemGrpcService stockItemGrpcService)
    {
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _stockItemGrpcService = stockItemGrpcService ?? throw new ArgumentNullException(nameof(stockItemGrpcService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet("{username}", Name = "GetBasket")]
    [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Cart>> GetBasket([Required] string username)
    {
        var cart = await _basketRepository.GetBasketByUsername(username);
        var result = _mapper.Map<CartDto>(cart) ?? new CartDto(username);

        return Ok(result);
    }

    [HttpPost(Name = "UpdateBasket")]
    [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CartDto>> UpdateBasket([FromBody] CartDto model)
    {
        // Communicate with Inventory.Grpc and check quantity available of products
        foreach (var item in model.Items)
        {
            var stock = await _stockItemGrpcService.GetStock(item.ItemNo);
            item.SetAvailableQuantity(stock.Quantity);
        }

        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(10));
        // .SetSlidingExpiration(TimeSpan.FromMinutes(5));

        var cart = _mapper.Map<Cart>(model);
        var updatedCart = await _basketRepository.UpdateBasket(cart, options);
        var result = _mapper.Map<CartDto>(updatedCart);

        return Ok(result);
    }

    [HttpDelete("{username}", Name = "DeleteBasket")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteBasket([Required] string username)
    {
        var result = await _basketRepository.DeleteBasketFromUsername(username);
        return Ok(result);
    }

    [Route("[action]/{username}")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Checkout([Required] string username, [FromBody] BasketCheckout basketCheckout)
    {
        var basket = await _basketRepository.GetBasketByUsername(basketCheckout.Username);
        if (basket == null || !basket.Items.Any()) return NotFound();

        // Publish checkout event to Eventbus Message
        var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        eventMessage.TotalPrice = basket.TotalPrice;
        await _publishEndpoint.Publish(eventMessage);

        // Remove the basket
        await _basketRepository.DeleteBasketFromUsername(basketCheckout.Username);

        return Accepted();
    }
}
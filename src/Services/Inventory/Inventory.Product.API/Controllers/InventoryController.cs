using System.ComponentModel.DataAnnotations;
using System.Net;
using Infrastructure.Common.Models;
using Inventory.Product.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Inventory;

namespace Inventory.Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// api/inventory/items/{itemNo}
    /// </summary>
    /// <returns></returns>
    [Route("items/{itemNo}", Name = "GetAllByItemNo")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntryDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<InventoryEntryDto>>> GetAllByItemNo([Required] string itemNo)
    {
        var result = await _inventoryService.GetAllByItemNoAsync(itemNo);
        return Ok(result);
    }

    /// <summary>
    /// api/inventory/items/{itemNo}/paging
    /// </summary>
    /// <returns></returns>
    [Route("items/{itemNo}/paging", Name = "GetAllByItemNoPaging")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<InventoryEntryDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PagedList<InventoryEntryDto>>> GetAllByItemNoPaging([Required] string itemNo,
        [FromQuery] GetInventoryPagingQuery query)
    {
        query.SetItemNo(itemNo);
        var result = await _inventoryService.GetAllByItemNoPagingAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// api/inventory/{id}
    /// </summary>
    /// <returns></returns>
    [Route("{id}", Name = "GetInventoryById")]
    [HttpGet]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> GetInventoryById([Required] string id)
    {
        var result = await _inventoryService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// api/inventory/purchase/{itemNo}
    /// </summary>
    /// <returns></returns>
    [HttpPost("purchase/{itemNo}", Name = "PurchaseOrder")]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> PurchaseOrder(
        [Required] string itemNo,
        [FromBody] PurchaseProductDto model)
    {
        var result = await _inventoryService.PurchaseItemAsync(itemNo, model);
        return Ok(result);
    }

    /// <summary>
    /// api/inventory/sales/{itemNo}
    /// </summary>
    /// <returns></returns>
    [HttpPost("sales/{itemNo}", Name = "SalesItem")]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> SalesItem(
        [Required] string itemNo,
        [FromBody] SalesProductDto model)
    {
        var result = await _inventoryService.SalesItemAsync(itemNo, model);
        return Ok(result);
    }

    /// <summary>
    /// api/inventory/sales/order-no/{orderNo}
    /// </summary>
    /// <param name="orderNo"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("sales/order-no/{orderNo}", Name = "SalesOrder")]
    [ProducesResponseType(typeof(CreatedSalesOrderSuccessDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CreatedSalesOrderSuccessDto>> SalesOrder(
        [Required] string orderNo,
        [FromBody] SalesOrderDto model)
    {
        model.OrderNo = orderNo;
        var documentNo = await _inventoryService.SalesOrderAsync(model);
        var result = new CreatedSalesOrderSuccessDto(documentNo);
        return Ok(result);
    }

    /// <summary>
    /// api/inventory/{id}
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{id}", Name = "DeleteById")]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> DeleteById(
        [Required] string id)
    {
        var entity = await _inventoryService.GetByIdAsync(id);
        if (entity == null) return NotFound();

        await _inventoryService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// api/inventory/document-no/{documentNo}
    /// </summary>
    /// <returns></returns>
    [HttpDelete("document-no/{documentNo}", Name = "DeleteByDocumentNo")]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> DeleteByDocumentNo(
        [Required] string documentNo)
    {
        await _inventoryService.DeleteByDocumentNoAsync(documentNo);
        return NoContent();
    }
}
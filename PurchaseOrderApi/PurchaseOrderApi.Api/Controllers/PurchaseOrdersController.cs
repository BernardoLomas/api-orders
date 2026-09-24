using Microsoft.AspNetCore.Mvc;
using PurchaseOrderApi.Api.Requests;
using PurchaseOrderApi.Application.PurchaseOrders.Create;
using PurchaseOrderApi.Application.PurchaseOrders.GetById;

namespace PurchaseOrderApi.Api.Controllers;

[ApiController]
[Route("purchase-orders")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly CreatePurchaseOrderHandler _handler;
    private readonly GetPurchaseOrderHandler _getHandler;

    public PurchaseOrdersController(CreatePurchaseOrderHandler handler, GetPurchaseOrderHandler getHandler)
    {
        _handler = handler;
        _getHandler = getHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new CreatePurchaseOrderCommand
        {
            SupplierId = request.SupplierId,
            Items = request.Items.Select(item => new CreatePurchaseOrderItemCommand
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };

        var result = await _handler.HandleAsync(command, cancellationToken);

        return Created($"/purchase-orders/{result.Id}", result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PurchaseOrderResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseOrderResult>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        PurchaseOrderResult? result = await _getHandler.HandleAsync(id, cancellationToken);

        if(result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
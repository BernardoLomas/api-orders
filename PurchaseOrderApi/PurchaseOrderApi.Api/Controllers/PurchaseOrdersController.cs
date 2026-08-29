using Microsoft.AspNetCore.Mvc;
using PurchaseOrderApi.Api.Requests;
using PurchaseOrderApi.Application.PurchaseOrders.Create;

namespace PurchaseOrderApi.Api.Controllers;

[ApiController]
[Route("purchase-orders")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly CreatePurchaseOrderHandler _handler;

    public PurchaseOrdersController(CreatePurchaseOrderHandler handler)
    {
        _handler = handler;
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

    [HttpGet]
    public async Task<IActionResult> CreatedAtAction([FromBody] )
}
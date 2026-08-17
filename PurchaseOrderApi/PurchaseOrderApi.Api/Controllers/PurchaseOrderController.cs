using Microsoft.AspNetCore.Mvc;
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
}
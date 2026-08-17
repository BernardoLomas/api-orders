using Microsoft.AspNetCore.Mvc;

namespace PurchaseOrderApi.Api.Controllers;

[ApiController]
[Route("purchase-orders")]

public class PurchaseOrderController : ControllerBase
{
    private readonly CreatePurchaseOrderHandler _handler;

    public PurchaseOrderController(CreatePurchaseOrderHandler handler)
    {
        _handler = handler;
    }
}

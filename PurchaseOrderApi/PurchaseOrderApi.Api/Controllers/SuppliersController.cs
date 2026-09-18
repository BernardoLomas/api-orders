using Microsoft.AspNetCore.Mvc;
using PurchaseOrderApi.Application.Suppliers.GetActive;

namespace PurchaseOrderApi.Api.Controllers;

[ApiController]
[Route("suppliers")]
public sealed class SuppliersController : ControllerBase
{
    private readonly GetActiveSuppliersHandler _handler;

    public SuppliersController(GetActiveSuppliersHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SupplierResult>>>GetActive(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<SupplierResult> suppliers = await _handler.HandleAsync(cancellationToken);
        
        return Ok(suppliers);
    }
}
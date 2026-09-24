using Microsoft.AspNetCore.Mvc;
using PurchaseOrderApi.Application.Suppliers.GetActive;
using PurchaseOrderApi.Application.Suppliers.GetProducts;

namespace PurchaseOrderApi.Api.Controllers;

[ApiController]
[Route("suppliers")]
public sealed class SuppliersController : ControllerBase
{
    private readonly GetActiveSuppliersHandler _getActiveSuppliersHandler;
    private readonly GetSupplierProductsHandler _getSupplierProductsHandler;

    public SuppliersController(GetActiveSuppliersHandler getActiveSuppliersHandler, GetSupplierProductsHandler getSupplierProductsHandler)
    {
        _getActiveSuppliersHandler = getActiveSuppliersHandler;
        _getSupplierProductsHandler = getSupplierProductsHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SupplierResult>>>GetActive(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<SupplierResult> suppliers = await _getActiveSuppliersHandler.HandleAsync(cancellationToken);
        
        return Ok(suppliers);
    }

    [HttpGet("{supplierId:guid}/products")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SupplierProductResult>),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<SupplierProductResult>>>GetProducts(Guid supplierId, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<SupplierProductResult>? supplierProducts = await _getSupplierProductsHandler.HandleAsync(supplierId, cancellationToken);

        if(supplierProducts is null)
        {
            return NotFound();
        }

        return Ok(supplierProducts);
    }
}
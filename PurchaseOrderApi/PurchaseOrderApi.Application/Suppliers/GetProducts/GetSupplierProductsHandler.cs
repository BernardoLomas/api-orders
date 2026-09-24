using PurchaseOrderApi.Application.Abstractions.Repositories;
using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Application.Suppliers.GetProducts;

public sealed class GetSupplierProductsHandler
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierProductRepository _supplierProductRepository;
    private readonly IProductRepository _productRepository;

    public GetSupplierProductsHandler(ISupplierRepository supplierRepository, ISupplierProductRepository supplierProductRepository, IProductRepository productRepository)
    {
        _supplierRepository = supplierRepository;
        _supplierProductRepository = supplierProductRepository;
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyCollection<SupplierProductResult>?> HandleAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        Supplier? supplier = await _supplierRepository.GetByIdAsync(supplierId, cancellationToken);

        if (supplier is null || !supplier.IsActive)
        {
            return null;
        }

        IReadOnlyCollection<SupplierProduct> supplierProducts = await _supplierProductRepository.GetActiveBySupplierIdAsync(supplierId, cancellationToken);

        List<SupplierProductResult> results = new();

        foreach (SupplierProduct supplierProduct in supplierProducts)
        {
            Product? product = await _productRepository.GetByIdAsync(supplierProduct.ProductId, cancellationToken);

            if (product is null || !product.IsActive)
            {
                continue;
            }
            
            results.Add(new SupplierProductResult( product.Id, product.Name, product.Description, supplierProduct.UnitPrice, supplierProduct.Currency ));
        }

        return results;

    }
}
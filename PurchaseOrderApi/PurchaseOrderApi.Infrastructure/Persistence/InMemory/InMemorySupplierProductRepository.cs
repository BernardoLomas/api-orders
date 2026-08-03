using PurchaseOrderApi.Application.Abstractions.Repositories;
using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Infrastructure.Persistence.InMemory
{
    public class InMemorySupplierProductRepository : ISupplierProductRepository
    {
        private readonly Dictionary<(Guid SupplierId, Guid ProductId), SupplierProduct> _supplierProducts = new();

        public Task<SupplierProduct?> GetBySupplierAndProductAsync(Guid supplierId, Guid productId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var key = (SupplierId: supplierId, ProductId: productId);

            _supplierProducts.TryGetValue(key, out SupplierProduct? supplierProduct);

            return Task.FromResult(supplierProduct);
        }

        public void Add(SupplierProduct supplierProduct)
        {
            ArgumentNullException.ThrowIfNull(supplierProduct);

            var key = (SupplierId: supplierProduct.SupplierId, ProductId: supplierProduct.ProductId);

            _supplierProducts[key] = supplierProduct;
        }
    }
}
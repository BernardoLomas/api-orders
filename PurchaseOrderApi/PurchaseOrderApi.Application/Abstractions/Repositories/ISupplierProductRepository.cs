using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Application.Abstractions.Repositories
{
    public interface ISupplierProductRepository
    {
        Task<SupplierProduct?> GetBySupplierAndProductAsync(Guid supplierId, Guid productId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<SupplierProduct>> GetActiveBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken = default); 
    }
}
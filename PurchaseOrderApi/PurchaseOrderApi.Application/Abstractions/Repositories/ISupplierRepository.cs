using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Application.Abstractions.Repositories
{
    public interface ISupplierRepository
    {
        Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Supplier>> GetActiveAsync(CancellationToken cancellationToken = default);
    }
}
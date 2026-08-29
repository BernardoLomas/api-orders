using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Application.Abstractions.Repositories
{
    public interface IPurchaseOrderRepository
    {
        Task AddAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
        Task<PurchaseOrder?> GetById(Guid id, CancellationToken cancellationToken = default);
    }
}
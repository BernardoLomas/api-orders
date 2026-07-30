using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Application.Abstractions.Repositories
{
    public interface IPurchaseOrderRepository
    {
        Task AddAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
    }
}
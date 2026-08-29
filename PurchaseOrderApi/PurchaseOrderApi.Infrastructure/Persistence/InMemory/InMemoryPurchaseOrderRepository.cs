using PurchaseOrderApi.Application.Abstractions.Repositories;
using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Infrastructure.Persistence.InMemory
{
    public class InMemoryPurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly Dictionary<Guid, PurchaseOrder> _purchaseOrders = new();

        public Task AddAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(purchaseOrder);

            _purchaseOrders[purchaseOrder.Id] = purchaseOrder;

            return Task.CompletedTask;
        }

        public Task<PurchaseOrder?> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _purchaseOrders.TryGetValue(id, out var purchaseOrder);
            return Task.FromResult(purchaseOrder);
        } 
    }
}
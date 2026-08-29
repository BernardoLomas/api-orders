using PurchaseOrderApi.Application.Abstractions.Repositories;
using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Application.PurchaseOrders.GetById
{
    public class GetPurchaseOrderHandler
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

        public GetPurchaseOrderHandler(IPurchaseOrderRepository purchaseOrderRepository)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        public async Task<PurchaseOrder?> HandleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentException("Purchase order ID is required.", nameof(id));
            }

            return await _purchaseOrderRepository.GetById(id, cancellationToken);
        }
    }
}
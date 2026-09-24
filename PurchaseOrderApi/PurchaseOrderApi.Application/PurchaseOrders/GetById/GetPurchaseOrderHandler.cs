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

        public async Task<PurchaseOrderResult?> HandleAsync(Guid id, CancellationToken cancellationToken = default)
        {   
            if(id == Guid.Empty)
            {
                throw new ArgumentException("Purchase order ID is required.", nameof(id));
            }

            PurchaseOrder? purchaseOrder = await _purchaseOrderRepository.GetById(id,cancellationToken);

            if(purchaseOrder is null)
            {
                return null;
            }

            List<PurchaseOrderItemResult> itemResults = purchaseOrder.Items.Select(
                item => new PurchaseOrderItemResult(
                    item.ProductId, 
                    item.Quantity, 
                    item.UnitPrice,
                    item.Currency, 
                    item.Total
                )
            ).ToList();

            return new PurchaseOrderResult(
                purchaseOrder.Id,
                purchaseOrder.SupplierId,
                purchaseOrder.PurchaseOrderStatus,
                purchaseOrder.Currency,
                purchaseOrder.Total,
                purchaseOrder.CreatedAt,
                purchaseOrder.UpdatedAt,
                itemResults
            ); 
        }
    }
}
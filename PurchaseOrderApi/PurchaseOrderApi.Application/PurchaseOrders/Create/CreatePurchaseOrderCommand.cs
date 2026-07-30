using PurchaseOrderApi.Domain.Entities;

namespace PurchaseOrderApi.Application.PurchaseOrders.Create
{
    public class CreatePurchaseOrder
    {
        public Guid SupplierId { get; init; }
        public List<CreatePurchaseOrderItemCommand> Items { get; init; } = new ();
    }
}
using PurchaseOrderApi.Domain.Enums;

namespace PurchaseOrderApi.Application.PurchaseOrders.Create
{
    public class CreatePurchaseOrderResult
    {
        public Guid Id { get; init; }
        public PurchaseOrderStatus PurchaseOrderStatus { get; init; }
        public required string Currency { get; init; }
        public decimal Total { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
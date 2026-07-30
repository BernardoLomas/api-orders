namespace PurchaseOrderApi.Application.PurchaseOrders.Create
{
    public class CreatePurchaseOrderItemCommand
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
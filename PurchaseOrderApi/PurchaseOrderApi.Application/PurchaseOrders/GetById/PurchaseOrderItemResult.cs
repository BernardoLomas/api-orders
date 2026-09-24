namespace PurchaseOrderApi.Application.PurchaseOrders.GetById;
public sealed record PurchaseOrderItemResult(Guid ProductId, int Quantity, decimal UnitPrice, string Currency, decimal Total);

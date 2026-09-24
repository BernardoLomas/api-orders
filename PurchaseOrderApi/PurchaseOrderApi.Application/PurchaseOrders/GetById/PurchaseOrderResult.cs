using PurchaseOrderApi.Domain.Enums;
namespace PurchaseOrderApi.Application.PurchaseOrders.GetById;

public sealed record PurchaseOrderResult(Guid Id, Guid SupplierId, PurchaseOrderStatus Status, string Currency, decimal Total, DateTime CreatedAt, DateTime UpdatedAt, IReadOnlyCollection<PurchaseOrderItemResult> Items);

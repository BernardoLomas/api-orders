 namespace PurchaseOrderApi.Api.Requests;

public sealed record CreatePurchaseOrderRequest(Guid SupplierId, List<CreatePurchaseOrderItemRequest> Items);
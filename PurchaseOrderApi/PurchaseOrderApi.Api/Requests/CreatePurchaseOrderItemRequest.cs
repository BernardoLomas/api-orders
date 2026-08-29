namespace PurchaseOrderApi.Api.Requests;
public sealed record CreatePurchaseOrderItemRequest(Guid ProductId, int Quantity);
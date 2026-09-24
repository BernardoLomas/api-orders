namespace PurchaseOrderApi.Application.Suppliers.GetProducts;

public sealed record SupplierProductResult(Guid ProductId, string Name, string Description, decimal UnitPrice, string Currency);

